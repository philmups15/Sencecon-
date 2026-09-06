using MediatR;
using Microsoft.EntityFrameworkCore;
using Sencecon.Application.Common.Interfaces;
using Sencecon.Domain.Enums;

namespace Sencecon.Application.Dashboard;

public record DashboardStageCountDto
{
    public LifecycleStage Stage { get; init; }
    public int Count { get; init; }
}

public record DashboardActivityDto
{
    public string Who { get; init; } = string.Empty;
    public string Action { get; init; } = string.Empty;
    public DateTimeOffset Created { get; init; }
}

public record DashboardAttentionDto
{
    public Guid PlantId { get; init; }
    public string Name { get; init; } = string.Empty;
    public PlantHealth Health { get; init; }
    public string Issue { get; init; } = string.Empty;
}

public record DashboardPlantLocationDto
{
    public Guid PlantId { get; init; }
    public string Name { get; init; } = string.Empty;
    public double Latitude { get; init; }
    public double Longitude { get; init; }
    public PlantHealth Health { get; init; }
    public LifecycleStage Stage { get; init; }
}

public record DashboardDto
{
    public int OperatingPlants { get; init; }
    public int InDelivery { get; init; }
    public int InCommissioning { get; init; }
    public int OpenWorkOrders { get; init; }
    public int SlaBreaching { get; init; }
    public decimal PipelineValue { get; init; }
    public int OpenOpportunities { get; init; }
    public IReadOnlyList<DashboardStageCountDto> StageDistribution { get; init; } = Array.Empty<DashboardStageCountDto>();
    public IReadOnlyList<DashboardActivityDto> RecentActivity { get; init; } = Array.Empty<DashboardActivityDto>();
    public IReadOnlyList<DashboardAttentionDto> AttentionPlants { get; init; } = Array.Empty<DashboardAttentionDto>();
    public IReadOnlyList<DashboardPlantLocationDto> PlantLocations { get; init; } = Array.Empty<DashboardPlantLocationDto>();
}

public record GetDashboardQuery : IRequest<DashboardDto>;

public class GetDashboardQueryHandler : IRequestHandler<GetDashboardQuery, DashboardDto>
{
    private readonly IApplicationDbContext _context;

    public GetDashboardQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<DashboardDto> Handle(GetDashboardQuery request, CancellationToken cancellationToken)
    {
        var now = DateTimeOffset.UtcNow;

        var plants = await _context.Plants
            .Where(p => p.IsActive)
            .Select(p => new { p.Id, p.Name, p.Stage, p.Health, p.PerformanceRatio, p.Latitude, p.Longitude })
            .ToListAsync(cancellationToken);

        var openWorkOrders = await _context.WorkOrders
            .Where(w => w.Status != WorkOrderStatus.Done)
            .Select(w => new { w.DueDate })
            .ToListAsync(cancellationToken);

        var openOpps = await _context.Opportunities
            .Where(o => o.Stage != OpportunityStage.Won && !o.Converted)
            .Select(o => o.Value)
            .ToListAsync(cancellationToken);

        var activity = await _context.AuditLogEntries
            .OrderByDescending(a => a.Created)
            .Take(8)
            .Select(a => new DashboardActivityDto
            {
                Who = a.User != null ? a.User.DisplayName : "System",
                Action = a.Action,
                Created = a.Created
            })
            .ToListAsync(cancellationToken);

        var attention = plants
            .Where(p => p.Health is PlantHealth.Watch or PlantHealth.AtRisk or PlantHealth.Critical)
            .OrderByDescending(p => p.Health)
            .Take(8)
            .Select(p => new DashboardAttentionDto
            {
                PlantId = p.Id,
                Name = p.Name,
                Health = p.Health,
                Issue = p.PerformanceRatio is { } pr && pr < 0.75
                    ? $"Performance ratio {pr * 100:0}%"
                    : $"Health flagged {p.Health}"
            })
            .ToList();

        return new DashboardDto
        {
            OperatingPlants = plants.Count(p => p.Stage == LifecycleStage.Operating),
            InDelivery = plants.Count(p => p.Stage is LifecycleStage.Deployment or LifecycleStage.Commissioning),
            InCommissioning = plants.Count(p => p.Stage == LifecycleStage.Commissioning),
            OpenWorkOrders = openWorkOrders.Count,
            SlaBreaching = openWorkOrders.Count(w => w.DueDate.HasValue && w.DueDate.Value < now),
            PipelineValue = openOpps.Sum(),
            OpenOpportunities = openOpps.Count,
            StageDistribution = Enum.GetValues<LifecycleStage>()
                .Select(s => new DashboardStageCountDto { Stage = s, Count = plants.Count(p => p.Stage == s) })
                .ToList(),
            RecentActivity = activity,
            AttentionPlants = attention,
            PlantLocations = plants
                .Where(p => p.Latitude.HasValue && p.Longitude.HasValue)
                .Select(p => new DashboardPlantLocationDto
                {
                    PlantId = p.Id, Name = p.Name, Latitude = p.Latitude!.Value, Longitude = p.Longitude!.Value,
                    Health = p.Health, Stage = p.Stage
                })
                .ToList()
        };
    }
}

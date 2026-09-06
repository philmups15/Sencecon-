using MediatR;
using Microsoft.EntityFrameworkCore;
using Sencecon.Application.Common.Interfaces;

namespace Sencecon.Application.Plants.Queries.GetPlants;

public record GetPlantsQuery : IRequest<IReadOnlyList<PlantDto>>
{
    public bool IncludeInactive { get; init; }
}

public class GetPlantsQueryHandler : IRequestHandler<GetPlantsQuery, IReadOnlyList<PlantDto>>
{
    private readonly IApplicationDbContext _context;

    public GetPlantsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<PlantDto>> Handle(GetPlantsQuery request, CancellationToken cancellationToken)
    {
        return await _context.Plants
            .Where(p => request.IncludeInactive || p.IsActive)
            .OrderBy(p => p.Code)
            .Select(p => new PlantDto
            {
                Id = p.Id,
                Code = p.Code,
                Name = p.Name,
                Stage = p.Stage,
                Type = p.Type,
                Capacity = p.Capacity,
                Equipment = p.Equipment,
                PerformanceRatio = p.PerformanceRatio,
                Latitude = p.Latitude,
                Longitude = p.Longitude,
                Health = p.Health,
                IsActive = p.IsActive,
                ProjectId = p.ProjectId,
                ProjectName = p.Project != null ? p.Project.Name : null,
                Created = p.Created
            })
            .ToListAsync(cancellationToken);
    }
}

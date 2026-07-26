using MediatR;
using Sencecon.Application.Common.Interfaces;
using Sencecon.Domain.Entities;
using Sencecon.Domain.Enums;

namespace Sencecon.Application.Plants.Commands.CreatePlant;

public record CreatePlantCommand : IRequest<Guid>
{
    public required string Code { get; init; }
    public required string Name { get; init; }
    public LifecycleStage Stage { get; init; }
    public string Capacity { get; init; } = string.Empty;
    public string Equipment { get; init; } = string.Empty;
    public double? PerformanceRatio { get; init; }
    public PlantHealth Health { get; init; }
}

public class CreatePlantCommandHandler : IRequestHandler<CreatePlantCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public CreatePlantCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreatePlantCommand request, CancellationToken cancellationToken)
    {
        var entity = new Plant
        {
            Code = request.Code,
            Name = request.Name,
            Stage = request.Stage,
            Capacity = request.Capacity,
            Equipment = request.Equipment,
            PerformanceRatio = request.PerformanceRatio,
            Health = request.Health,
            Created = DateTimeOffset.UtcNow
        };

        _context.Plants.Add(entity);

        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}

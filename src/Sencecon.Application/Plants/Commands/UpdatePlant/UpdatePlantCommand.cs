using MediatR;
using Microsoft.EntityFrameworkCore;
using Sencecon.Application.Common.Interfaces;
using Sencecon.Domain.Enums;
using Sencecon.Domain.Exceptions;

namespace Sencecon.Application.Plants.Commands.UpdatePlant;

public record UpdatePlantCommand : IRequest
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public LifecycleStage Stage { get; init; }
    public PlantType Type { get; init; }
    public string Capacity { get; init; } = string.Empty;
    public string Equipment { get; init; } = string.Empty;
    public double? PerformanceRatio { get; init; }
    public double? Latitude { get; init; }
    public double? Longitude { get; init; }
    public PlantHealth Health { get; init; }
    public Guid? ProjectId { get; init; }
}

public class UpdatePlantCommandHandler : IRequestHandler<UpdatePlantCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdatePlantCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(UpdatePlantCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Plants
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        if (entity is null)
        {
            throw new NotFoundException(nameof(Domain.Entities.Plant), request.Id);
        }

        if (request.ProjectId.HasValue)
        {
            var projectExists = await _context.Projects
                .AnyAsync(p => p.Id == request.ProjectId.Value, cancellationToken);

            if (!projectExists)
            {
                throw new NotFoundException(nameof(Domain.Entities.Project), request.ProjectId.Value);
            }
        }

        entity.Name = request.Name;
        entity.Stage = request.Stage;
        entity.Type = request.Type;
        entity.Capacity = request.Capacity;
        entity.Equipment = request.Equipment;
        entity.PerformanceRatio = request.PerformanceRatio;
        entity.Latitude = request.Latitude;
        entity.Longitude = request.Longitude;
        entity.Health = request.Health;
        entity.ProjectId = request.ProjectId;
        entity.LastModified = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
    }
}

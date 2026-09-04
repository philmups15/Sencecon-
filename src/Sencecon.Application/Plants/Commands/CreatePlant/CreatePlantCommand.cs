using MediatR;
using Microsoft.EntityFrameworkCore;
using Sencecon.Application.Common;
using Sencecon.Application.Common.Interfaces;
using Sencecon.Domain.Entities;
using Sencecon.Domain.Enums;
using Sencecon.Domain.Exceptions;

namespace Sencecon.Application.Plants.Commands.CreatePlant;

public record CreatePlantCommand : IRequest<Guid>
{
    public required string Name { get; init; }
    public LifecycleStage Stage { get; init; }
    public string Capacity { get; init; } = string.Empty;
    public string Equipment { get; init; } = string.Empty;
    public double? PerformanceRatio { get; init; }
    public PlantHealth Health { get; init; }
    public Guid? ProjectId { get; init; }
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
        if (request.ProjectId.HasValue)
        {
            var projectExists = await _context.Projects
                .AnyAsync(p => p.Id == request.ProjectId.Value, cancellationToken);

            if (!projectExists)
            {
                throw new NotFoundException(nameof(Domain.Entities.Project), request.ProjectId.Value);
            }
        }

        var code = await EntityCodeGenerator.GenerateNextCodeAsync(_context.Plants.Select(p => p.Code), "PLT-", cancellationToken);

        var entity = new Plant
        {
            Code = code,
            Name = request.Name,
            Stage = request.Stage,
            Capacity = request.Capacity,
            Equipment = request.Equipment,
            PerformanceRatio = request.PerformanceRatio,
            Health = request.Health,
            ProjectId = request.ProjectId,
            Created = DateTimeOffset.UtcNow
        };

        _context.Plants.Add(entity);

        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}

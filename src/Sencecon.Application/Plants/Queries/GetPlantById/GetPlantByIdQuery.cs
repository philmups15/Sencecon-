using MediatR;
using Microsoft.EntityFrameworkCore;
using Sencecon.Application.Common.Interfaces;
using Sencecon.Application.Plants.Queries.GetPlants;
using Sencecon.Domain.Exceptions;

namespace Sencecon.Application.Plants.Queries.GetPlantById;

public record GetPlantByIdQuery : IRequest<PlantDto>
{
    public required Guid Id { get; init; }
}

public class GetPlantByIdQueryHandler : IRequestHandler<GetPlantByIdQuery, PlantDto>
{
    private readonly IApplicationDbContext _context;

    public GetPlantByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PlantDto> Handle(GetPlantByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _context.Plants
            .Include(p => p.Project)
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        if (entity is null)
        {
            throw new NotFoundException(nameof(Domain.Entities.Plant), request.Id);
        }

        return new PlantDto
        {
            Id = entity.Id,
            Code = entity.Code,
            Name = entity.Name,
            Stage = entity.Stage,
            Type = entity.Type,
            Capacity = entity.Capacity,
            Equipment = entity.Equipment,
            PerformanceRatio = entity.PerformanceRatio,
            Latitude = entity.Latitude,
            Longitude = entity.Longitude,
            Health = entity.Health,
            IsActive = entity.IsActive,
            ProjectId = entity.ProjectId,
            ProjectName = entity.Project?.Name,
            Created = entity.Created
        };
    }
}

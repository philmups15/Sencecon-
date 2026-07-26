using MediatR;
using Microsoft.EntityFrameworkCore;
using Sencecon.Application.Common.Interfaces;

namespace Sencecon.Application.Plants.Queries.GetPlants;

public record GetPlantsQuery : IRequest<IReadOnlyList<PlantDto>>;

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
            .OrderBy(p => p.Code)
            .Select(p => new PlantDto
            {
                Id = p.Id,
                Code = p.Code,
                Name = p.Name,
                Stage = p.Stage,
                Capacity = p.Capacity,
                Equipment = p.Equipment,
                PerformanceRatio = p.PerformanceRatio,
                Health = p.Health,
                Created = p.Created
            })
            .ToListAsync(cancellationToken);
    }
}

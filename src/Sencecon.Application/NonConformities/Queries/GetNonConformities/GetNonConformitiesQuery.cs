using MediatR;
using Microsoft.EntityFrameworkCore;
using Sencecon.Application.Common.Interfaces;

namespace Sencecon.Application.NonConformities.Queries.GetNonConformities;

public record GetNonConformitiesQuery : IRequest<IReadOnlyList<NonConformityDto>>;

public class GetNonConformitiesQueryHandler : IRequestHandler<GetNonConformitiesQuery, IReadOnlyList<NonConformityDto>>
{
    private readonly IApplicationDbContext _context;

    public GetNonConformitiesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<NonConformityDto>> Handle(GetNonConformitiesQuery request, CancellationToken cancellationToken)
    {
        return await _context.NonConformities
            .OrderByDescending(n => n.Created)
            .Select(n => new NonConformityDto
            {
                Id = n.Id,
                Code = n.Code,
                Description = n.Description,
                PlantName = n.Plant != null ? n.Plant.Name : n.PlantName,
                Status = n.Status,
                PlantId = n.PlantId,
                Created = n.Created
            })
            .ToListAsync(cancellationToken);
    }
}

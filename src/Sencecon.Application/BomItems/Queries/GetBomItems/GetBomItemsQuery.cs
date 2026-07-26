using MediatR;
using Microsoft.EntityFrameworkCore;
using Sencecon.Application.Common.Interfaces;

namespace Sencecon.Application.BomItems.Queries.GetBomItems;

public record GetBomItemsQuery : IRequest<IReadOnlyList<BomItemDto>>;

public class GetBomItemsQueryHandler : IRequestHandler<GetBomItemsQuery, IReadOnlyList<BomItemDto>>
{
    private readonly IApplicationDbContext _context;

    public GetBomItemsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<BomItemDto>> Handle(GetBomItemsQuery request, CancellationToken cancellationToken)
    {
        return await _context.BomItems
            .OrderByDescending(b => b.Created)
            .Select(b => new BomItemDto
            {
                Id = b.Id,
                Component = b.Component,
                Quantity = b.Quantity,
                UnitCost = b.UnitCost,
                Supplier = b.Supplier,
                Status = b.Status,
                Created = b.Created
            })
            .ToListAsync(cancellationToken);
    }
}

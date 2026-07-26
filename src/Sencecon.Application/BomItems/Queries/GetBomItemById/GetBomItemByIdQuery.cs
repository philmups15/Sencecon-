using MediatR;
using Microsoft.EntityFrameworkCore;
using Sencecon.Application.BomItems.Queries.GetBomItems;
using Sencecon.Application.Common.Interfaces;
using Sencecon.Domain.Exceptions;

namespace Sencecon.Application.BomItems.Queries.GetBomItemById;

public record GetBomItemByIdQuery : IRequest<BomItemDto>
{
    public required Guid Id { get; init; }
}

public class GetBomItemByIdQueryHandler : IRequestHandler<GetBomItemByIdQuery, BomItemDto>
{
    private readonly IApplicationDbContext _context;

    public GetBomItemByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<BomItemDto> Handle(GetBomItemByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _context.BomItems
            .FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);

        if (entity is null)
        {
            throw new NotFoundException(nameof(Domain.Entities.BomItem), request.Id);
        }

        return new BomItemDto
        {
            Id = entity.Id,
            Component = entity.Component,
            Quantity = entity.Quantity,
            UnitCost = entity.UnitCost,
            Supplier = entity.Supplier,
            Status = entity.Status,
            Created = entity.Created
        };
    }
}

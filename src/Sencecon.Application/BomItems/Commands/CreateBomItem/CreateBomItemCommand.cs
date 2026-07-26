using MediatR;
using Sencecon.Application.Common.Interfaces;
using Sencecon.Domain.Entities;
using Sencecon.Domain.Enums;

namespace Sencecon.Application.BomItems.Commands.CreateBomItem;

public record CreateBomItemCommand : IRequest<Guid>
{
    public required string Component { get; init; }
    public int Quantity { get; init; }
    public decimal UnitCost { get; init; }
    public string Supplier { get; init; } = string.Empty;
    public BomStatus Status { get; init; }
}

public class CreateBomItemCommandHandler : IRequestHandler<CreateBomItemCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public CreateBomItemCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreateBomItemCommand request, CancellationToken cancellationToken)
    {
        var entity = new BomItem
        {
            Component = request.Component,
            Quantity = request.Quantity,
            UnitCost = request.UnitCost,
            Supplier = request.Supplier,
            Status = request.Status,
            Created = DateTimeOffset.UtcNow
        };

        _context.BomItems.Add(entity);

        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}

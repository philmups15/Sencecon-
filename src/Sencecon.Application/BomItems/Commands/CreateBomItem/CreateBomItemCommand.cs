using MediatR;
using Microsoft.EntityFrameworkCore;
using Sencecon.Application.Common.Interfaces;
using Sencecon.Domain.Entities;
using Sencecon.Domain.Enums;
using Sencecon.Domain.Exceptions;

namespace Sencecon.Application.BomItems.Commands.CreateBomItem;

public record CreateBomItemCommand : IRequest<Guid>
{
    public required string Component { get; init; }
    public int Quantity { get; init; }
    public decimal UnitCost { get; init; }
    public string Supplier { get; init; } = string.Empty;
    public BomStatus Status { get; init; }
    public Guid? PlantId { get; init; }
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
        if (request.PlantId.HasValue)
        {
            var plantExists = await _context.Plants
                .AnyAsync(p => p.Id == request.PlantId.Value, cancellationToken);

            if (!plantExists)
            {
                throw new NotFoundException(nameof(Domain.Entities.Plant), request.PlantId.Value);
            }
        }

        var entity = new BomItem
        {
            Component = request.Component,
            Quantity = request.Quantity,
            UnitCost = request.UnitCost,
            Supplier = request.Supplier,
            Status = request.Status,
            PlantId = request.PlantId,
            Created = DateTimeOffset.UtcNow
        };

        _context.BomItems.Add(entity);

        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}

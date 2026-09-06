using MediatR;
using Microsoft.EntityFrameworkCore;
using Sencecon.Application.Common.Interfaces;
using Sencecon.Domain.Enums;
using Sencecon.Domain.Exceptions;

namespace Sencecon.Application.BomItems.Commands.UpdateBomItem;

public record UpdateBomItemCommand : IRequest
{
    public required Guid Id { get; init; }
    public required string Component { get; init; }
    public BomCategory Category { get; init; }
    public int Quantity { get; init; }
    public decimal UnitCost { get; init; }
    public string Supplier { get; init; } = string.Empty;
    public BomStatus Status { get; init; }
    public Guid? PlantId { get; init; }
    public Guid? ProjectId { get; init; }
}

public class UpdateBomItemCommandHandler : IRequestHandler<UpdateBomItemCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateBomItemCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(UpdateBomItemCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.BomItems
            .FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);

        if (entity is null)
        {
            throw new NotFoundException(nameof(Domain.Entities.BomItem), request.Id);
        }

        if (request.PlantId.HasValue && !await _context.Plants.AnyAsync(p => p.Id == request.PlantId.Value, cancellationToken))
            throw new NotFoundException(nameof(Domain.Entities.Plant), request.PlantId.Value);

        if (request.ProjectId.HasValue && !await _context.Projects.AnyAsync(p => p.Id == request.ProjectId.Value, cancellationToken))
            throw new NotFoundException(nameof(Domain.Entities.Project), request.ProjectId.Value);

        entity.Component = request.Component;
        entity.Category = request.Category;
        entity.Quantity = request.Quantity;
        entity.UnitCost = request.UnitCost;
        entity.Supplier = request.Supplier;
        entity.Status = request.Status;
        entity.PlantId = request.PlantId;
        entity.ProjectId = request.ProjectId;
        entity.LastModified = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
    }
}

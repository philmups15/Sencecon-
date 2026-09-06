using MediatR;
using Microsoft.EntityFrameworkCore;
using Sencecon.Application.Common.Interfaces;
using Sencecon.Domain.Enums;
using Sencecon.Domain.Exceptions;

namespace Sencecon.Application.WorkOrders.Commands.UpdateWorkOrder;

public record UpdateWorkOrderCommand : IRequest
{
    public required Guid Id { get; init; }
    public required string Title { get; init; }
    public WorkOrderType Type { get; init; }
    public Priority Priority { get; init; }
    public string Assignee { get; init; } = string.Empty;
    public WorkOrderStatus Status { get; init; }
    public DateTimeOffset? DueDate { get; init; }
    public required Guid PlantId { get; init; }
}

public class UpdateWorkOrderCommandHandler : IRequestHandler<UpdateWorkOrderCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateWorkOrderCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(UpdateWorkOrderCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.WorkOrders
            .FirstOrDefaultAsync(w => w.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.WorkOrder), request.Id);

        if (!await _context.Plants.AnyAsync(p => p.Id == request.PlantId, cancellationToken))
            throw new NotFoundException(nameof(Domain.Entities.Plant), request.PlantId);

        // Sign-off gate: a work order with a checklist can only close once every
        // item is ticked.
        if (request.Status == WorkOrderStatus.Done && entity.Status != WorkOrderStatus.Done)
        {
            var checklist = await _context.WorkOrderChecklistItems
                .Where(c => c.WorkOrderId == entity.Id)
                .Select(c => c.IsDone)
                .ToListAsync(cancellationToken);

            if (checklist.Count > 0 && checklist.Any(done => !done))
            {
                var outstanding = checklist.Count(done => !done);
                throw new ConflictException($"Cannot close this work order — {outstanding} checklist item(s) are not complete.");
            }
        }

        entity.Title = request.Title;
        entity.Type = request.Type;
        entity.Priority = request.Priority;
        entity.Assignee = request.Assignee;
        entity.Status = request.Status;
        entity.DueDate = request.DueDate;
        entity.PlantId = request.PlantId;
        entity.LastModified = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
    }
}

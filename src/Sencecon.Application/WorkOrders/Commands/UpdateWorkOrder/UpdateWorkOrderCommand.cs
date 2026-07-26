using MediatR;
using Microsoft.EntityFrameworkCore;
using Sencecon.Application.Common.Interfaces;
using Sencecon.Domain.Enums;
using Sencecon.Domain.Exceptions;

namespace Sencecon.Application.WorkOrders.Commands.UpdateWorkOrder;

public record UpdateWorkOrderCommand : IRequest
{
    public required Guid Id { get; init; }
    public required string Code { get; init; }
    public required string Title { get; init; }
    public WorkOrderType Type { get; init; }
    public Priority Priority { get; init; }
    public string Assignee { get; init; } = string.Empty;
    public WorkOrderStatus Status { get; init; }
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
            .FirstOrDefaultAsync(w => w.Id == request.Id, cancellationToken);

        if (entity is null)
        {
            throw new NotFoundException(nameof(Domain.Entities.WorkOrder), request.Id);
        }

        var plantExists = await _context.Plants
            .AnyAsync(p => p.Id == request.PlantId, cancellationToken);

        if (!plantExists)
        {
            throw new NotFoundException(nameof(Domain.Entities.Plant), request.PlantId);
        }

        entity.Code = request.Code;
        entity.Title = request.Title;
        entity.Type = request.Type;
        entity.Priority = request.Priority;
        entity.Assignee = request.Assignee;
        entity.Status = request.Status;
        entity.PlantId = request.PlantId;
        entity.LastModified = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
    }
}

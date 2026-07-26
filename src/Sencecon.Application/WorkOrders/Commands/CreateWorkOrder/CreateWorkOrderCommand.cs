using MediatR;
using Microsoft.EntityFrameworkCore;
using Sencecon.Application.Common.Interfaces;
using Sencecon.Domain.Entities;
using Sencecon.Domain.Enums;
using Sencecon.Domain.Exceptions;

namespace Sencecon.Application.WorkOrders.Commands.CreateWorkOrder;

public record CreateWorkOrderCommand : IRequest<Guid>
{
    public required string Code { get; init; }
    public required string Title { get; init; }
    public WorkOrderType Type { get; init; }
    public Priority Priority { get; init; }
    public string Assignee { get; init; } = string.Empty;
    public WorkOrderStatus Status { get; init; }
    public required Guid PlantId { get; init; }
}

public class CreateWorkOrderCommandHandler : IRequestHandler<CreateWorkOrderCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public CreateWorkOrderCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreateWorkOrderCommand request, CancellationToken cancellationToken)
    {
        var plantExists = await _context.Plants
            .AnyAsync(p => p.Id == request.PlantId, cancellationToken);

        if (!plantExists)
        {
            throw new NotFoundException(nameof(Domain.Entities.Plant), request.PlantId);
        }

        var entity = new WorkOrder
        {
            Code = request.Code,
            Title = request.Title,
            Type = request.Type,
            Priority = request.Priority,
            Assignee = request.Assignee,
            Status = request.Status,
            PlantId = request.PlantId,
            Created = DateTimeOffset.UtcNow
        };

        _context.WorkOrders.Add(entity);

        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}

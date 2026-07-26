using MediatR;
using Microsoft.EntityFrameworkCore;
using Sencecon.Application.Common.Interfaces;
using Sencecon.Application.WorkOrders.Queries.GetWorkOrders;
using Sencecon.Domain.Exceptions;

namespace Sencecon.Application.WorkOrders.Queries.GetWorkOrderById;

public record GetWorkOrderByIdQuery : IRequest<WorkOrderDto>
{
    public required Guid Id { get; init; }
}

public class GetWorkOrderByIdQueryHandler : IRequestHandler<GetWorkOrderByIdQuery, WorkOrderDto>
{
    private readonly IApplicationDbContext _context;

    public GetWorkOrderByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<WorkOrderDto> Handle(GetWorkOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _context.WorkOrders
            .Include(w => w.Plant)
            .FirstOrDefaultAsync(w => w.Id == request.Id, cancellationToken);

        if (entity is null)
        {
            throw new NotFoundException(nameof(Domain.Entities.WorkOrder), request.Id);
        }

        return new WorkOrderDto
        {
            Id = entity.Id,
            Code = entity.Code,
            Title = entity.Title,
            Type = entity.Type,
            Priority = entity.Priority,
            Assignee = entity.Assignee,
            Status = entity.Status,
            PlantId = entity.PlantId,
            PlantName = entity.Plant!.Name,
            Created = entity.Created
        };
    }
}

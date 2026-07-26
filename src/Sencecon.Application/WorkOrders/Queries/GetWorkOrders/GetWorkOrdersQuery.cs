using MediatR;
using Microsoft.EntityFrameworkCore;
using Sencecon.Application.Common.Interfaces;

namespace Sencecon.Application.WorkOrders.Queries.GetWorkOrders;

public record GetWorkOrdersQuery : IRequest<IReadOnlyList<WorkOrderDto>>;

public class GetWorkOrdersQueryHandler : IRequestHandler<GetWorkOrdersQuery, IReadOnlyList<WorkOrderDto>>
{
    private readonly IApplicationDbContext _context;

    public GetWorkOrdersQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<WorkOrderDto>> Handle(GetWorkOrdersQuery request, CancellationToken cancellationToken)
    {
        return await _context.WorkOrders
            .OrderBy(w => w.Code)
            .Select(w => new WorkOrderDto
            {
                Id = w.Id,
                Code = w.Code,
                Title = w.Title,
                Type = w.Type,
                Priority = w.Priority,
                Assignee = w.Assignee,
                Status = w.Status,
                PlantId = w.PlantId,
                PlantName = w.Plant!.Name,
                Created = w.Created
            })
            .ToListAsync(cancellationToken);
    }
}

using Sencecon.Domain.Enums;

namespace Sencecon.Application.WorkOrders.Queries.GetWorkOrders;

public record WorkOrderDto
{
    public Guid Id { get; init; }
    public string Code { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public WorkOrderType Type { get; init; }
    public Priority Priority { get; init; }
    public string Assignee { get; init; } = string.Empty;
    public WorkOrderStatus Status { get; init; }
    public Guid PlantId { get; init; }
    public string PlantName { get; init; } = string.Empty;
    public DateTimeOffset Created { get; init; }
}

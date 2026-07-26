using Sencecon.Domain.Common;
using Sencecon.Domain.Enums;

namespace Sencecon.Domain.Entities;

public class WorkOrder : BaseAuditableEntity
{
    public string Code { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public WorkOrderType Type { get; set; }
    public Priority Priority { get; set; }
    public string Assignee { get; set; } = string.Empty;
    public WorkOrderStatus Status { get; set; } = WorkOrderStatus.Open;

    public Guid PlantId { get; set; }
    public Plant? Plant { get; set; }
}

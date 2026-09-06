using Sencecon.Domain.Common;

namespace Sencecon.Domain.Entities;

public class WorkOrderPart : BaseEntity
{
    public Guid WorkOrderId { get; set; }
    public WorkOrder WorkOrder { get; set; } = null!;
    public string PartName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public string? Notes { get; set; }
}

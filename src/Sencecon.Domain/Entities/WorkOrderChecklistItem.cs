using Sencecon.Domain.Common;

namespace Sencecon.Domain.Entities;

public class WorkOrderChecklistItem : BaseEntity
{
    public Guid WorkOrderId { get; set; }
    public WorkOrder WorkOrder { get; set; } = null!;
    public string Text { get; set; } = string.Empty;
    public bool IsDone { get; set; }
    public int Order { get; set; }
}

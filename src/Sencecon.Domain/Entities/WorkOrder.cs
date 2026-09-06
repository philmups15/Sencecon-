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
    public DateTimeOffset? DueDate { get; set; }

    public Guid PlantId { get; set; }
    public Plant? Plant { get; set; }

    public ICollection<WorkOrderChecklistItem> ChecklistItems { get; set; } = new List<WorkOrderChecklistItem>();
    public ICollection<WorkOrderPart> Parts { get; set; } = new List<WorkOrderPart>();
    public ICollection<WorkOrderLabour> Labour { get; set; } = new List<WorkOrderLabour>();
    public ICollection<WorkOrderAttachment> Attachments { get; set; } = new List<WorkOrderAttachment>();
}

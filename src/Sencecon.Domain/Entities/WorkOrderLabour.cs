using Sencecon.Domain.Common;

namespace Sencecon.Domain.Entities;

public class WorkOrderLabour : BaseEntity
{
    public Guid WorkOrderId { get; set; }
    public WorkOrder WorkOrder { get; set; } = null!;
    public string PersonName { get; set; } = string.Empty;
    public decimal Hours { get; set; }
    public DateTimeOffset? WorkDate { get; set; }
    public string? Notes { get; set; }
}

using Sencecon.Domain.Common;
using Sencecon.Domain.Enums;

namespace Sencecon.Domain.Entities;

public class CommissioningTestResult : BaseAuditableEntity
{
    public Guid PlantId { get; set; }
    public Plant Plant { get; set; } = null!;
    public CommissioningTestCategory Category { get; set; }
    public string TestName { get; set; } = string.Empty;
    public CommissioningResultStatus Result { get; set; } = CommissioningResultStatus.Pending;
    public string? Notes { get; set; }
    public Guid? RecordedBy { get; set; }
}

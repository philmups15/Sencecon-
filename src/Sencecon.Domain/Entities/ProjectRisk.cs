using Sencecon.Domain.Common;
using Sencecon.Domain.Enums;

namespace Sencecon.Domain.Entities;

public class ProjectRisk : BaseAuditableEntity
{
    public Guid ProjectId { get; set; }
    public Project Project { get; set; } = null!;
    public string Description { get; set; } = string.Empty;
    public RiskSeverity Severity { get; set; } = RiskSeverity.Medium;
    public string Mitigation { get; set; } = string.Empty;
    public RiskStatus Status { get; set; } = RiskStatus.Open;
}

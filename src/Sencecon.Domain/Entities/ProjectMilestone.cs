using Sencecon.Domain.Common;
using Sencecon.Domain.Enums;

namespace Sencecon.Domain.Entities;

public class ProjectMilestone : BaseAuditableEntity
{
    public Guid ProjectId { get; set; }
    public Project Project { get; set; } = null!;
    public string Label { get; set; } = string.Empty;
    public MilestoneState State { get; set; } = MilestoneState.Upcoming;
    public int Order { get; set; }
}

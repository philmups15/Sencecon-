using Sencecon.Domain.Common;
using Sencecon.Domain.Enums;

namespace Sencecon.Domain.Entities;

public class Project : BaseAuditableEntity
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Customer { get; set; } = string.Empty;
    public LifecycleStage Stage { get; set; }
    public string ProjectManager { get; set; } = string.Empty;
    public decimal Budget { get; set; }
    public decimal Actual { get; set; }

    public bool IsActive { get; set; } = true;
    public DateTimeOffset? ScheduledStartDate { get; set; }
    public DateTimeOffset? ScheduledEndDate { get; set; }

    public ICollection<Survey> Surveys { get; set; } = new List<Survey>();
    public ICollection<Plant> Plants { get; set; } = new List<Plant>();
    public ICollection<Design> Designs { get; set; } = new List<Design>();
    public ICollection<BomItem> BomItems { get; set; } = new List<BomItem>();

    public ICollection<ProjectMilestone> Milestones { get; set; } = new List<ProjectMilestone>();
    public ICollection<ProjectTask> Tasks { get; set; } = new List<ProjectTask>();
    public ICollection<Subcontractor> Subcontractors { get; set; } = new List<Subcontractor>();
    public ICollection<ProjectRisk> Risks { get; set; } = new List<ProjectRisk>();
    public ICollection<ProjectBudgetLine> BudgetLines { get; set; } = new List<ProjectBudgetLine>();
}

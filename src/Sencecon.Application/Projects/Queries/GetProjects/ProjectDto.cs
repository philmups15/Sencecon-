using Sencecon.Domain.Enums;
using Sencecon.Application.Projects.Queries.GetProjectById;

namespace Sencecon.Application.Projects.Queries.GetProjects;

public record ProjectDto
{
    public Guid Id { get; init; }
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string Customer { get; init; } = string.Empty;
    public LifecycleStage Stage { get; init; }
    public string ProjectManager { get; init; } = string.Empty;
    public decimal Budget { get; init; }
    public decimal Actual { get; init; }
    public DateTimeOffset Created { get; init; }

    // Populated by GetProjectByIdQuery only.
    public IReadOnlyList<ProjectMilestoneDto>? Milestones { get; init; }
    public IReadOnlyList<ProjectTaskDto>? Tasks { get; init; }
    public IReadOnlyList<SubcontractorDto>? Subcontractors { get; init; }
    public IReadOnlyList<ProjectRiskDto>? Risks { get; init; }
    public IReadOnlyList<ProjectBudgetLineDto>? BudgetLines { get; init; }
}

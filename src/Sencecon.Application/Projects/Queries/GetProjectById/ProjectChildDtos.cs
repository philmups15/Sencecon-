using Sencecon.Domain.Enums;

namespace Sencecon.Application.Projects.Queries.GetProjectById;

public record ProjectMilestoneDto
{
    public Guid Id { get; init; }
    public string Label { get; init; } = string.Empty;
    public MilestoneState State { get; init; }
    public int Order { get; init; }
}

public record ProjectTaskDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Owner { get; init; } = string.Empty;
    public DateTimeOffset? DueDate { get; init; }
    public ProjectTaskStatus Status { get; init; }
}

public record SubcontractorDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Scope { get; init; } = string.Empty;
    public SubcontractorStatus Status { get; init; }
}

public record ProjectRiskDto
{
    public Guid Id { get; init; }
    public string Description { get; init; } = string.Empty;
    public RiskSeverity Severity { get; init; }
    public string Mitigation { get; init; } = string.Empty;
    public RiskStatus Status { get; init; }
}

public record ProjectBudgetLineDto
{
    public Guid Id { get; init; }
    public string Label { get; init; } = string.Empty;
    public decimal BudgetAmount { get; init; }
    public decimal ActualAmount { get; init; }
    public BomCategory? Category { get; init; }
}

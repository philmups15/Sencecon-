using Sencecon.Domain.Enums;

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
}

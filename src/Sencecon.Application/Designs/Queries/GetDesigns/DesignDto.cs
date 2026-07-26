using Sencecon.Domain.Enums;

namespace Sencecon.Application.Designs.Queries.GetDesigns;

public record DesignDto
{
    public Guid Id { get; init; }
    public string Code { get; init; } = string.Empty;
    public string ProjectName { get; init; } = string.Empty;
    public DesignStatus Status { get; init; }
    public string Revision { get; init; } = string.Empty;
    public Guid? SurveyId { get; init; }
    public string? SurveyCode { get; init; }
    public DateTimeOffset Created { get; init; }
}

using Sencecon.Domain.Enums;

namespace Sencecon.Application.Surveys.Queries.GetSurveys;

public record SurveyDto
{
    public Guid Id { get; init; }
    public string Code { get; init; } = string.Empty;
    public string PlantName { get; init; } = string.Empty;
    public SurveyStatus Status { get; init; }
    public int Progress { get; init; }
    public string Surveyor { get; init; } = string.Empty;
    public DateTimeOffset Date { get; init; }
}

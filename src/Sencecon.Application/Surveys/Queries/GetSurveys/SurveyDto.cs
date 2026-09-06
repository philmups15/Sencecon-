using Sencecon.Domain.Enums;
using Sencecon.Application.Surveys.Commands.UploadSurveyPhotos;

namespace Sencecon.Application.Surveys.Queries.GetSurveys;

public record SurveyMeasurementDto
{
    public Guid Id { get; init; }
    public string Field { get; init; } = string.Empty;
    public string Value { get; init; } = string.Empty;
}

public record SurveyObstructionDto
{
    public Guid Id { get; init; }
    public string Item { get; init; } = string.Empty;
    public string Impact { get; init; } = string.Empty;
}

public record SurveyDto
{
    public Guid Id { get; init; }
    public string Code { get; init; } = string.Empty;
    public string PlantName { get; init; } = string.Empty;
    public SurveyStatus Status { get; init; }
    public int Progress { get; init; }
    public string Surveyor { get; init; } = string.Empty;
    public DateTimeOffset Date { get; init; }
    public Guid? ProjectId { get; init; }
    public string? ProjectName { get; init; }
    public Guid? PlantId { get; init; }

    // GetSurveyByIdQuery only.
    public IReadOnlyList<SurveyMeasurementDto>? Measurements { get; init; }
    public IReadOnlyList<SurveyObstructionDto>? Obstructions { get; init; }
    public IReadOnlyList<SurveyPhotoDto>? Photos { get; init; }
}

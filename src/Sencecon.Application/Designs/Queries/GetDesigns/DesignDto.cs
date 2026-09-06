using Sencecon.Domain.Enums;
using Sencecon.Application.Designs.Queries.GetDesignAttachments;

namespace Sencecon.Application.Designs.Queries.GetDesigns;

public record DesignRevisionDto
{
    public Guid Id { get; init; }
    public string Revision { get; init; } = string.Empty;
    public string? Note { get; init; }
    public string? ChangedByName { get; init; }
    public DateTimeOffset Created { get; init; }
}

public record DesignDto
{
    public Guid Id { get; init; }
    public string Code { get; init; } = string.Empty;
    public string ProjectName { get; init; } = string.Empty;
    public DesignStatus Status { get; init; }
    public string Revision { get; init; } = string.Empty;
    public Guid? SurveyId { get; init; }
    public string? SurveyCode { get; init; }
    public Guid? ProjectId { get; init; }
    public DateTimeOffset Created { get; init; }

    // GetDesignByIdQuery only.
    public Dictionary<string, Dictionary<string, string>>? Specs { get; init; }
    public IReadOnlyList<DesignAttachmentDto>? Attachments { get; init; }
    public IReadOnlyList<DesignRevisionDto>? Revisions { get; init; }
}

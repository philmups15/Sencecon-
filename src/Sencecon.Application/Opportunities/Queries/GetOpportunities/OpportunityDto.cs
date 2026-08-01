using Sencecon.Domain.Enums;

namespace Sencecon.Application.Opportunities.Queries.GetOpportunities;

public record OpportunityDto
{
    public Guid Id { get; init; }
    public string Code { get; init; } = string.Empty;
    public string Customer { get; init; } = string.Empty;
    public string Capacity { get; init; } = string.Empty;
    public OpportunityStage Stage { get; init; }
    public string Location { get; init; } = string.Empty;
    public string NextAction { get; init; } = string.Empty;
    public string Owner { get; init; } = string.Empty;
    public decimal Value { get; init; }
    public string? Notes { get; init; }
    public Guid CreatedBy { get; init; }
    public string CreatedByName { get; init; } = string.Empty;
    public DateTimeOffset Created { get; init; }
    public IReadOnlyList<OpportunityAttachmentDto> Attachments { get; init; } = Array.Empty<OpportunityAttachmentDto>();
}

public record OpportunityAttachmentDto
{
    public Guid Id { get; init; }
    public string FileName { get; init; } = string.Empty;
    public string ContentType { get; init; } = string.Empty;
    public long SizeBytes { get; init; }
    public Guid UploadedBy { get; init; }
    public string UploadedByName { get; init; } = string.Empty;
    public DateTimeOffset Created { get; init; }
}

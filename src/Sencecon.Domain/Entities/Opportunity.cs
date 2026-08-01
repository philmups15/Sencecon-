using Sencecon.Domain.Common;
using Sencecon.Domain.Enums;

namespace Sencecon.Domain.Entities;

public class Opportunity : BaseAuditableEntity
{
    public string Code { get; set; } = string.Empty;
    public string Customer { get; set; } = string.Empty;
    public string Capacity { get; set; } = string.Empty;
    public OpportunityStage Stage { get; set; }
    public string Location { get; set; } = string.Empty;
    public string NextAction { get; set; } = string.Empty;
    public string Owner { get; set; } = string.Empty;
    public decimal Value { get; set; }
    public string? Notes { get; set; }
    public Guid CreatedBy { get; set; }

    public ICollection<OpportunityAttachment> Attachments { get; set; } = new List<OpportunityAttachment>();
}

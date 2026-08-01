using Sencecon.Domain.Common;

namespace Sencecon.Domain.Entities;

public class OpportunityNote : BaseEntity
{
    public Guid OpportunityId { get; set; }
    public Opportunity Opportunity { get; set; } = null!;
    public string Text { get; set; } = string.Empty;
    public Guid CreatedBy { get; set; }
    public DateTimeOffset Created { get; set; }
}

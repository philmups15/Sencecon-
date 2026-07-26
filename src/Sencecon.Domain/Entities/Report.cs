using Sencecon.Domain.Common;

namespace Sencecon.Domain.Entities;

public class Report : BaseAuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string GeneratedBy { get; set; } = string.Empty;
    public DateTimeOffset GeneratedDate { get; set; }
}

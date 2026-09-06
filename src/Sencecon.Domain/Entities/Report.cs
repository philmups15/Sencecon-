using Sencecon.Domain.Common;
using Sencecon.Domain.Enums;

namespace Sencecon.Domain.Entities;

public class Report : BaseAuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public ReportType Type { get; set; }
    public string GeneratedBy { get; set; } = string.Empty;
    public DateTimeOffset GeneratedDate { get; set; }
}

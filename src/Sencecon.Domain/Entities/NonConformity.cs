using Sencecon.Domain.Common;
using Sencecon.Domain.Enums;

namespace Sencecon.Domain.Entities;

public class NonConformity : BaseAuditableEntity
{
    public string Code { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string PlantName { get; set; } = string.Empty;
    public NonConformityStatus Status { get; set; }
}

using Sencecon.Domain.Common;
using Sencecon.Domain.Enums;

namespace Sencecon.Domain.Entities;

public class CommissioningTestTemplate : BaseAuditableEntity
{
    public CommissioningTestCategory Category { get; set; }
    public string TestName { get; set; } = string.Empty;
    // Plant types this test applies to (stored as a Postgres integer[]).
    // Empty means "all types".
    public int[] AppliesToTypes { get; set; } = Array.Empty<int>();
    public int Order { get; set; }
    public bool IsActive { get; set; } = true;
}

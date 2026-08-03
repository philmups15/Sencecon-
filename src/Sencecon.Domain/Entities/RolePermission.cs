using Sencecon.Domain.Common;

namespace Sencecon.Domain.Entities;

// The 5 roles themselves are still a fixed backend enum (UserRole) — this table
// makes what each role can do per module editable, replacing the previously
// hardcoded matrix in Sencecon.API/Authorization/Roles.cs. One row per
// (Role, Module) pair; Role/Module are stored as strings rather than FKs since
// both are fixed, small, code-defined sets (see RolePermissionCatalogue).
public class RolePermission : BaseAuditableEntity
{
    public string Role { get; set; } = string.Empty;
    public string Module { get; set; } = string.Empty;
    public bool CanRead { get; set; }
    public bool CanWrite { get; set; }
}

namespace Sencecon.Application.RolePermissions.Queries.GetRolePermissions;

public record RolePermissionDto
{
    public required string Role { get; init; }
    public required string Module { get; init; }
    public required bool CanRead { get; init; }
    public required bool CanWrite { get; init; }
}

namespace Sencecon.API.Authorization;

public static class Roles
{
    public const string Admin = "Admin";
    public const string User = "User";
    public const string ProjectManager = "ProjectManager";
    public const string Sales = "Sales";
    public const string DesignEngineer = "DesignEngineer";
}

// Per-module read/write access used to be a hardcoded matrix here
// (ModuleAccess). It's now the RolePermission table, editable via
// PUT /api/role-permissions/{role}/{module} — see ModulePermissionRequirement
// for the runtime check and RolePermissionConfiguration for the seed data that
// matches what this matrix used to say. Roles.Admin above is still used
// directly for a handful of admin-only actions (user management, audit log,
// integrations) that aren't part of the per-module matrix.

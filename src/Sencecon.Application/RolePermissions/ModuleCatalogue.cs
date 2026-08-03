namespace Sencecon.Application.RolePermissions;

// The fixed set of modules whose access is governed by RolePermission rows.
// Keys match the frontend's MODULE_ACCESS keys in src/lib/api.js exactly, and
// the module list previously hardcoded in Sencecon.API/Authorization/Roles.cs.
// Adding a module here doesn't do anything by itself — a controller action
// still needs [Authorize(Policy = "{key}-read"/"{key}-write")], see Program.cs.
public static class ModuleCatalogue
{
    public static readonly IReadOnlyList<string> Keys =
    [
        "opportunities",
        "surveys",
        "designs",
        "bomItems",
        "projects",
        "plants",
        "workOrders",
        "nonConformities",
        "reports",
    ];

    public static bool IsKnown(string module) => Keys.Contains(module);
}

using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Sencecon.Application.Common.Interfaces;

namespace Sencecon.API.Authorization;

// Backs policies named "{module}-read" / "{module}-write" (registered in
// Program.cs, one pair per Sencecon.Application.RolePermissions.ModuleCatalogue
// entry) — replaces the old static [Authorize(Roles = ModuleAccess.XxxRead)]
// attributes with a runtime lookup against the RolePermissions table, so an
// admin editing the Roles & Permissions page actually changes what the API
// enforces, not just what the UI displays.
public class ModulePermissionRequirement : IAuthorizationRequirement
{
    public string Module { get; }
    public string Mode { get; } // "read" or "write"

    public ModulePermissionRequirement(string module, string mode)
    {
        Module = module;
        Mode = mode;
    }
}

public class ModulePermissionAuthorizationHandler : AuthorizationHandler<ModulePermissionRequirement>
{
    private readonly IApplicationDbContext _context;

    public ModulePermissionAuthorizationHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, ModulePermissionRequirement requirement)
    {
        var role = context.User.FindFirst(ClaimTypes.Role)?.Value;
        if (role is null)
        {
            return;
        }

        var permission = await _context.RolePermissions
            .FirstOrDefaultAsync(p => p.Role == role && p.Module == requirement.Module);

        var allowed = requirement.Mode == "write" ? permission?.CanWrite ?? false : permission?.CanRead ?? false;

        if (allowed)
        {
            context.Succeed(requirement);
        }
    }
}

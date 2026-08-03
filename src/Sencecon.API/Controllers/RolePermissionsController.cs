using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sencecon.API.Authorization;
using Sencecon.Application.RolePermissions.Commands.UpdateRolePermission;
using Sencecon.Application.RolePermissions.Queries.GetRolePermissions;

namespace Sencecon.API.Controllers;

// Any authenticated user can view the matrix (mirrors the old read-only
// Roles & Permissions page); only Admins can change it.
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RolePermissionsController : ControllerBase
{
    private readonly ISender _sender;

    public RolePermissionsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<RolePermissionDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<RolePermissionDto>>> GetAll()
    {
        var result = await _sender.Send(new GetRolePermissionsQuery());
        return Ok(result);
    }

    [HttpPut("{role}/{module}")]
    [Authorize(Roles = Roles.Admin)]
    [ProducesResponseType(typeof(RolePermissionDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<RolePermissionDto>> Update(string role, string module, UpdateRolePermissionRequest request)
    {
        var result = await _sender.Send(new UpdateRolePermissionCommand
        {
            Role = role,
            Module = module,
            CanRead = request.CanRead,
            CanWrite = request.CanWrite
        });

        return Ok(result);
    }
}

public record UpdateRolePermissionRequest(bool CanRead, bool CanWrite);

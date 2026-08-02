using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sencecon.API.Authorization;
using Sencecon.Application.Common.Interfaces;
using Sencecon.Application.Users.Commands.AdminSetPassword;
using Sencecon.Application.Users.Commands.ChangePassword;
using Sencecon.Application.Users.Commands.SendPasswordReset;
using Sencecon.Application.Users.Commands.SetUserStatus;
using Sencecon.Application.Users.Commands.UpdateProfile;
using Sencecon.Application.Users.Commands.UpdateUserRole;
using Sencecon.Application.Users.Queries.GetCurrentUser;
using Sencecon.Application.Users.Queries.GetUsers;
using Sencecon.Domain.Enums;

namespace Sencecon.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly ISender _sender;
    private readonly ICurrentUserService _currentUserService;

    public UsersController(ISender sender, ICurrentUserService currentUserService)
    {
        _sender = sender;
        _currentUserService = currentUserService;
    }

    private Guid CurrentUserId => _currentUserService.UserId
        ?? throw new UnauthorizedAccessException("No authenticated user.");

    [HttpGet]
    [Authorize(Roles = Roles.Admin)]
    [ProducesResponseType(typeof(IReadOnlyList<UserDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<UserDto>>> GetAll()
    {
        var result = await _sender.Send(new GetUsersQuery());
        return Ok(result);
    }

    [HttpGet("me")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<UserDto>> GetMe()
    {
        var result = await _sender.Send(new GetCurrentUserQuery { UserId = CurrentUserId });
        return Ok(result);
    }

    [HttpPut("me")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<UserDto>> UpdateMe(UpdateProfileRequest request)
    {
        var result = await _sender.Send(new UpdateProfileCommand
        {
            UserId = CurrentUserId,
            DisplayName = request.DisplayName
        });

        return Ok(result);
    }

    [HttpPut("me/password")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> ChangePassword(ChangePasswordRequest request)
    {
        await _sender.Send(new ChangePasswordCommand
        {
            UserId = CurrentUserId,
            CurrentPassword = request.CurrentPassword,
            NewPassword = request.NewPassword
        });

        return NoContent();
    }

    [HttpPut("{id:guid}/role")]
    [Authorize(Roles = Roles.Admin)]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<UserDto>> UpdateRole(Guid id, UpdateUserRoleRequest request)
    {
        var result = await _sender.Send(new UpdateUserRoleCommand
        {
            UserId = id,
            Role = request.Role,
            RequestingUserId = CurrentUserId
        });

        return Ok(result);
    }

    [HttpPut("{id:guid}/status")]
    [Authorize(Roles = Roles.Admin)]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<UserDto>> SetStatus(Guid id, SetUserStatusRequest request)
    {
        var result = await _sender.Send(new SetUserStatusCommand
        {
            UserId = id,
            Enabled = request.Enabled,
            RequestingUserId = CurrentUserId
        });

        return Ok(result);
    }

    [HttpPut("{id:guid}/password")]
    [Authorize(Roles = Roles.Admin)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> AdminSetPassword(Guid id, AdminSetPasswordRequest request)
    {
        await _sender.Send(new AdminSetPasswordCommand
        {
            UserId = id,
            NewPassword = request.NewPassword,
            RequestingUserId = CurrentUserId
        });

        return NoContent();
    }

    [HttpPost("{id:guid}/password-reset")]
    [Authorize(Roles = Roles.Admin)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> SendPasswordReset(Guid id)
    {
        await _sender.Send(new SendPasswordResetCommand
        {
            UserId = id,
            RequestingUserId = CurrentUserId
        });

        return NoContent();
    }
}

public record UpdateProfileRequest(string DisplayName);

public record ChangePasswordRequest(string CurrentPassword, string NewPassword);

public record UpdateUserRoleRequest(UserRole Role);

public record SetUserStatusRequest(bool Enabled);

public record AdminSetPasswordRequest(string NewPassword);

using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sencecon.Application.Common.Interfaces;
using Sencecon.Application.Users.Commands.ChangePassword;
using Sencecon.Application.Users.Commands.UpdateProfile;
using Sencecon.Application.Users.Queries.GetCurrentUser;
using Sencecon.Application.Users.Queries.GetUsers;

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
}

public record UpdateProfileRequest(string DisplayName);

public record ChangePasswordRequest(string CurrentPassword, string NewPassword);

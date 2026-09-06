using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sencecon.Application.Auth.Commands.Login;
using Sencecon.Application.Auth.Commands.Register;
using Sencecon.Application.Auth.Commands.RequestPasswordReset;
using Sencecon.Application.Auth.Commands.ResetPassword;
using Sencecon.Application.Auth.Common;

namespace Sencecon.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public class AuthController : ControllerBase
{
    private readonly ISender _sender;

    public AuthController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResult), StatusCodes.Status200OK)]
    public async Task<ActionResult<AuthResult>> Register(RegisterCommand command)
    {
        var result = await _sender.Send(command);
        return Ok(result);
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResult), StatusCodes.Status200OK)]
    public async Task<ActionResult<AuthResult>> Login(LoginCommand command)
    {
        var result = await _sender.Send(command with
        {
            IpAddress = ClientIp(),
            UserAgent = Request.Headers.UserAgent.ToString() is { Length: > 0 } ua ? ua : null
        });
        return Ok(result);
    }

    // Railway terminates TLS at a proxy, so the real client IP is the first hop
    // in X-Forwarded-For; fall back to the socket address for local runs.
    private string? ClientIp()
    {
        var forwarded = Request.Headers["X-Forwarded-For"].ToString();
        if (!string.IsNullOrWhiteSpace(forwarded))
        {
            return forwarded.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)[0];
        }
        return HttpContext.Connection.RemoteIpAddress?.ToString();
    }

    // Self-service "forgot my password" — takes an email, always returns 204 so
    // it can't be used to probe which addresses have accounts.
    [HttpPost("forgot-password")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> ForgotPassword(RequestPasswordResetCommand command)
    {
        await _sender.Send(command);
        return NoContent();
    }

    [HttpPost("reset-password")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> ResetPassword(ResetPasswordCommand command)
    {
        await _sender.Send(command);
        return NoContent();
    }
}

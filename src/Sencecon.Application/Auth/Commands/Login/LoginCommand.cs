using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sencecon.Application.Auth.Common;
using Sencecon.Application.Common.Interfaces;

namespace Sencecon.Application.Auth.Commands.Login;

public record LoginCommand : IRequest<AuthResult>
{
    public required string Email { get; init; }
    public required string Password { get; init; }

    // Filled in by the API from the HTTP request, not the client body — used only
    // for the "new sign-in" alert email.
    public string? IpAddress { get; init; }
    public string? UserAgent { get; init; }
}

public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResult>
{
    private readonly IApplicationDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IEmailService _emailService;
    private readonly ILogger<LoginCommandHandler> _logger;

    public LoginCommandHandler(
        IApplicationDbContext context,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator,
        IEmailService emailService,
        ILogger<LoginCommandHandler> logger)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task<AuthResult> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == normalizedEmail, cancellationToken);

        if (user is null || !_passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Invalid email or password.");
        }

        if (!user.IsActive)
        {
            throw new UnauthorizedAccessException("This account has been disabled.");
        }

        var token = _jwtTokenGenerator.GenerateToken(user);

        // Best-effort "you just signed in" alert. A mail outage or missing
        // Resend config must never block a valid login, so swallow everything.
        try
        {
            await _emailService.SendLoginAlertEmailAsync(
                user.Email, user.DisplayName, DateTimeOffset.UtcNow,
                request.IpAddress, request.UserAgent, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to send login-alert email for user {UserId}", user.Id);
        }

        return new AuthResult
        {
            UserId = user.Id,
            Email = user.Email,
            Token = token
        };
    }
}

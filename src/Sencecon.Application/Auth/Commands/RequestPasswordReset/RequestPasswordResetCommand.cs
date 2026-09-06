using System.Security.Cryptography;
using System.Text;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sencecon.Application.Common.Interfaces;
using Sencecon.Domain.Entities;

namespace Sencecon.Application.Auth.Commands.RequestPasswordReset;

// Self-service "forgot password": the visitor supplies only an email address.
// The response is always the same (204) whether or not that email has an
// account, so this endpoint can't be used to discover which addresses are
// registered.
public record RequestPasswordResetCommand : IRequest
{
    public required string Email { get; init; }
}

public class RequestPasswordResetCommandValidator : AbstractValidator<RequestPasswordResetCommand>
{
    public RequestPasswordResetCommandValidator()
    {
        RuleFor(v => v.Email).NotEmpty().EmailAddress();
    }
}

public class RequestPasswordResetCommandHandler : IRequestHandler<RequestPasswordResetCommand>
{
    private static readonly TimeSpan TokenLifetime = TimeSpan.FromHours(24);

    private readonly IApplicationDbContext _context;
    private readonly IEmailService _emailService;
    private readonly ILogger<RequestPasswordResetCommandHandler> _logger;

    public RequestPasswordResetCommandHandler(
        IApplicationDbContext context,
        IEmailService emailService,
        ILogger<RequestPasswordResetCommandHandler> logger)
    {
        _context = context;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task Handle(RequestPasswordResetCommand request, CancellationToken cancellationToken)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == normalizedEmail, cancellationToken);

        // Unknown or disabled account: do nothing, but return normally so the
        // caller can't tell the difference.
        if (user is null || !user.IsActive)
        {
            return;
        }

        var rawToken = Convert.ToHexString(RandomNumberGenerator.GetBytes(32));
        var tokenHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(rawToken)));

        _context.PasswordResetTokens.Add(new PasswordResetToken
        {
            UserId = user.Id,
            TokenHash = tokenHash,
            ExpiresAt = DateTimeOffset.UtcNow.Add(TokenLifetime),
            Used = false,
            Created = DateTimeOffset.UtcNow
        });

        await _context.SaveChangesAsync(cancellationToken);

        try
        {
            await _emailService.SendPasswordResetEmailAsync(user.Email, user.DisplayName, rawToken, cancellationToken);
        }
        catch (Exception ex)
        {
            // The token is already saved; surfacing the send failure to an
            // anonymous caller would leak configuration state. Log and move on.
            _logger.LogError(ex, "Failed to send password-reset email for user {UserId}", user.Id);
        }
    }
}

using System.Security.Cryptography;
using System.Text;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Sencecon.Application.Common.Interfaces;
using Sencecon.Domain.Exceptions;

namespace Sencecon.Application.Auth.Commands.ResetPassword;

public record ResetPasswordCommand : IRequest
{
    public required string Token { get; init; }
    public required string NewPassword { get; init; }
}

public class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordCommandValidator()
    {
        RuleFor(v => v.Token).NotEmpty();
        RuleFor(v => v.NewPassword)
            .NotEmpty()
            .MinimumLength(8)
            .MaximumLength(128);
    }
}

public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IPasswordHasher _passwordHasher;

    public ResetPasswordCommandHandler(IApplicationDbContext context, IPasswordHasher passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    public async Task Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var tokenHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(request.Token)));
        var now = DateTimeOffset.UtcNow;

        var resetToken = await _context.PasswordResetTokens
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.TokenHash == tokenHash, cancellationToken);

        // Deliberately the same error whether the token doesn't exist, is expired,
        // or was already used — don't give an attacker information about which.
        if (resetToken is null || resetToken.Used || resetToken.ExpiresAt < now || resetToken.User is null)
        {
            throw new ConflictException("This reset link is invalid or has expired.");
        }

        resetToken.User.PasswordHash = _passwordHasher.Hash(request.NewPassword);
        resetToken.User.LastModified = now;
        resetToken.Used = true;
        resetToken.LastModified = now;

        await _context.SaveChangesAsync(cancellationToken);
    }
}

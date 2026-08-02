using System.Security.Cryptography;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Sencecon.Application.Common.Interfaces;
using Sencecon.Domain.Entities;
using Sencecon.Domain.Exceptions;

namespace Sencecon.Application.Users.Commands.SendPasswordReset;

public record SendPasswordResetCommand : IRequest
{
    public required Guid UserId { get; init; }
    public required Guid RequestingUserId { get; init; }
}

public class SendPasswordResetCommandHandler : IRequestHandler<SendPasswordResetCommand>
{
    private static readonly TimeSpan TokenLifetime = TimeSpan.FromHours(24);

    private readonly IApplicationDbContext _context;
    private readonly IEmailService _emailService;

    public SendPasswordResetCommandHandler(IApplicationDbContext context, IEmailService emailService)
    {
        _context = context;
        _emailService = emailService;
    }

    public async Task Handle(SendPasswordResetCommand request, CancellationToken cancellationToken)
    {
        if (request.UserId == request.RequestingUserId)
        {
            throw new ConflictException("You cannot send yourself a reset link here. Use your profile's change-password instead.");
        }

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

        if (user is null)
        {
            throw new NotFoundException(nameof(Domain.Entities.User), request.UserId);
        }

        var rawToken = Convert.ToHexString(RandomNumberGenerator.GetBytes(32));
        var tokenHash = Convert.ToHexString(SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(rawToken)));

        _context.PasswordResetTokens.Add(new PasswordResetToken
        {
            UserId = user.Id,
            TokenHash = tokenHash,
            ExpiresAt = DateTimeOffset.UtcNow.Add(TokenLifetime),
            Used = false,
            Created = DateTimeOffset.UtcNow
        });

        await _context.SaveChangesAsync(cancellationToken);

        await _emailService.SendPasswordResetEmailAsync(user.Email, user.DisplayName, rawToken, cancellationToken);
    }
}

using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Sencecon.Application.Common.Interfaces;
using Sencecon.Domain.Exceptions;

namespace Sencecon.Application.Users.Commands.AdminSetPassword;

public record AdminSetPasswordCommand : IRequest
{
    public required Guid UserId { get; init; }
    public required string NewPassword { get; init; }
    public required Guid RequestingUserId { get; init; }
}

public class AdminSetPasswordCommandValidator : AbstractValidator<AdminSetPasswordCommand>
{
    public AdminSetPasswordCommandValidator()
    {
        RuleFor(v => v.NewPassword)
            .NotEmpty()
            .MinimumLength(8)
            .MaximumLength(128);
    }
}

// Distinct from ChangePasswordCommand: this is an admin setting *someone else's*
// password directly, so there's no current-password check — the admin's own
// authentication (JWT + [Authorize(Roles = Roles.Admin)]) is the gate instead.
public class AdminSetPasswordCommandHandler : IRequestHandler<AdminSetPasswordCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IPasswordHasher _passwordHasher;

    public AdminSetPasswordCommandHandler(IApplicationDbContext context, IPasswordHasher passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    public async Task Handle(AdminSetPasswordCommand request, CancellationToken cancellationToken)
    {
        if (request.UserId == request.RequestingUserId)
        {
            throw new ConflictException("You cannot set your own password here. Use your profile's change-password instead.");
        }

        var entity = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

        if (entity is null)
        {
            throw new NotFoundException(nameof(Domain.Entities.User), request.UserId);
        }

        entity.PasswordHash = _passwordHasher.Hash(request.NewPassword);
        entity.LastModified = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
    }
}

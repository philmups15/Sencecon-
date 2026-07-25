using MediatR;
using Microsoft.EntityFrameworkCore;
using Sencecon.Application.Auth.Common;
using Sencecon.Application.Common.Interfaces;
using Sencecon.Domain.Entities;
using Sencecon.Domain.Exceptions;

namespace Sencecon.Application.Auth.Commands.Register;

public record RegisterCommand : IRequest<AuthResult>
{
    public required string Email { get; init; }
    public required string Password { get; init; }
    public required string DisplayName { get; init; }
}

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResult>
{
    private readonly IApplicationDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public RegisterCommandHandler(
        IApplicationDbContext context,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<AuthResult> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();

        var emailInUse = await _context.Users
            .AnyAsync(u => u.Email == normalizedEmail, cancellationToken);

        if (emailInUse)
        {
            throw new ConflictException($"Email \"{request.Email}\" is already registered.");
        }

        var user = new User
        {
            Email = normalizedEmail,
            DisplayName = request.DisplayName,
            PasswordHash = _passwordHasher.Hash(request.Password),
            Created = DateTimeOffset.UtcNow
        };

        _context.Users.Add(user);

        await _context.SaveChangesAsync(cancellationToken);

        var token = _jwtTokenGenerator.GenerateToken(user);

        return new AuthResult
        {
            UserId = user.Id,
            Email = user.Email,
            Token = token
        };
    }
}

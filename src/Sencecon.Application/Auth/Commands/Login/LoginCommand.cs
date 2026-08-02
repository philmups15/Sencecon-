using MediatR;
using Microsoft.EntityFrameworkCore;
using Sencecon.Application.Auth.Common;
using Sencecon.Application.Common.Interfaces;

namespace Sencecon.Application.Auth.Commands.Login;

public record LoginCommand : IRequest<AuthResult>
{
    public required string Email { get; init; }
    public required string Password { get; init; }
}

public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResult>
{
    private readonly IApplicationDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public LoginCommandHandler(
        IApplicationDbContext context,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
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

        return new AuthResult
        {
            UserId = user.Id,
            Email = user.Email,
            Token = token
        };
    }
}

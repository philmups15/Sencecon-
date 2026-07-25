namespace Sencecon.Application.Auth.Common;

public record AuthResult
{
    public required Guid UserId { get; init; }
    public required string Email { get; init; }
    public required string Token { get; init; }
}

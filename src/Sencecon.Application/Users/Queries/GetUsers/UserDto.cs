using Sencecon.Domain.Enums;

namespace Sencecon.Application.Users.Queries.GetUsers;

public record UserDto
{
    public Guid Id { get; init; }
    public string Email { get; init; } = string.Empty;
    public string DisplayName { get; init; } = string.Empty;
    public UserRole Role { get; init; }
    public DateTimeOffset Created { get; init; }
}

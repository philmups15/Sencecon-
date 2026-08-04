using Sencecon.Domain.Entities;
using Sencecon.Domain.Enums;

namespace Sencecon.Application.Users.Queries.GetUsers;

public record UserDto
{
    public Guid Id { get; init; }
    public string Email { get; init; } = string.Empty;
    public string DisplayName { get; init; } = string.Empty;
    public UserRole Role { get; init; }
    public bool IsActive { get; init; }
    public DateTimeOffset Created { get; init; }

    public string? Username { get; init; }
    public string? PhoneNumber { get; init; }
    public string? Address { get; init; }
    public string? JobDescription { get; init; }

    // Never inline the actual image bytes into this DTO — fetch via
    // GET /api/users/{id}/avatar instead. This just tells the frontend
    // whether there's anything to fetch.
    public bool HasAvatar { get; init; }
}

public static class UserDtoMapper
{
    // Used everywhere a User entity needs to become a UserDto (GetUsers,
    // GetCurrentUser, UpdateProfile, UpdateUserRole, SetUserStatus) — keeps
    // all 10 fields in one place instead of five slightly-drifting copies.
    public static UserDto ToDto(this User user) => new()
    {
        Id = user.Id,
        Email = user.Email,
        DisplayName = user.DisplayName,
        Role = user.Role,
        IsActive = user.IsActive,
        Created = user.Created,
        Username = user.Username,
        PhoneNumber = user.PhoneNumber,
        Address = user.Address,
        JobDescription = user.JobDescription,
        HasAvatar = user.AvatarContent is { Length: > 0 },
    };
}

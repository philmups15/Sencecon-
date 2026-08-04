using Sencecon.Domain.Common;
using Sencecon.Domain.Enums;

namespace Sencecon.Domain.Entities;

public class User : BaseAuditableEntity
{
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.User;
    public bool IsActive { get; set; } = true;

    // Profile details — all optional, filled in at registration and/or later
    // via the Profile page. None of these are used for authentication (Email
    // remains the login identifier).
    public string? Username { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public string? JobDescription { get; set; }

    // Avatar stored as DB bytea, same pattern as OpportunityAttachment.Content —
    // avatars are small and singular (unlike attachments, no versioning needed).
    public byte[]? AvatarContent { get; set; }
    public string? AvatarContentType { get; set; }
    public string? AvatarFileName { get; set; }

    public ICollection<TodoItem> TodoItems { get; set; } = new List<TodoItem>();
    public ICollection<AuditLogEntry> AuditLogEntries { get; set; } = new List<AuditLogEntry>();
    public ICollection<PasswordResetToken> PasswordResetTokens { get; set; } = new List<PasswordResetToken>();
}

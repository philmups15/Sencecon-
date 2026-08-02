using Sencecon.Domain.Common;

namespace Sencecon.Domain.Entities;

public class PasswordResetToken : BaseAuditableEntity
{
    public Guid UserId { get; set; }
    public User? User { get; set; }

    // SHA-256 hash of the raw token — the raw value is only ever emailed to the
    // user, never stored, so a DB leak alone can't be used to reset an account.
    public string TokenHash { get; set; } = string.Empty;
    public DateTimeOffset ExpiresAt { get; set; }
    public bool Used { get; set; }
}

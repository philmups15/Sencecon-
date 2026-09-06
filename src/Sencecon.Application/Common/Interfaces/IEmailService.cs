namespace Sencecon.Application.Common.Interfaces;

public interface IEmailService
{
    // Deliberately doesn't take a pre-built link — building the reset URL needs
    // the frontend's base address, which is an Infrastructure-layer concern
    // (Application shouldn't know the frontend's deployment URL). The
    // implementation owns the link + email template; Application only decides
    // *that* a reset email should be sent, not what it looks like.
    Task SendPasswordResetEmailAsync(string toEmail, string displayName, string rawToken, CancellationToken cancellationToken);

    // Sent to the account holder every time a sign-in succeeds, so an unexpected
    // login is visible to them. Best-effort — callers must not fail the login if
    // this throws. ipAddress / userAgent are whatever the API could see on the
    // request and may be null.
    Task SendLoginAlertEmailAsync(string toEmail, string displayName, DateTimeOffset whenUtc, string? ipAddress, string? userAgent, CancellationToken cancellationToken);
}

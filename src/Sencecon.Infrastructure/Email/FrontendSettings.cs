namespace Sencecon.Infrastructure.Email;

// The base URL of the deployed frontend SPA, used to build the link inside a
// password-reset email. Not JWT/Cors-related, but small enough not to warrant
// its own top-level settings namespace.
public class FrontendSettings
{
    public const string SectionName = "Frontend";

    public required string BaseUrl { get; init; }
}

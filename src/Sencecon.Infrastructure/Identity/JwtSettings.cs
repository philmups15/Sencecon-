namespace Sencecon.Infrastructure.Identity;

public class JwtSettings
{
    public const string SectionName = "Jwt";

    public required string Issuer { get; init; }
    public required string Audience { get; init; }
    public required string Secret { get; init; }
    public int ExpiryMinutes { get; init; } = 60;
}

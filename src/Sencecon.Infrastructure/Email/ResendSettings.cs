namespace Sencecon.Infrastructure.Email;

public class ResendSettings
{
    public const string SectionName = "Resend";

    public required string ApiKey { get; init; }
    public required string FromAddress { get; init; }
}

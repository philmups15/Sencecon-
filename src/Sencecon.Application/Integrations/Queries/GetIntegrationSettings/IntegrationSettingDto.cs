namespace Sencecon.Application.Integrations.Queries.GetIntegrationSettings;

public record IntegrationSettingDto
{
    public required string Key { get; init; }
    public required string Name { get; init; }
    public required string Status { get; init; }
    public string? ProviderEndpoint { get; init; }
    public bool HasApiKey { get; init; }
    public string? Notes { get; init; }
    public DateTimeOffset? LastModified { get; init; }
}

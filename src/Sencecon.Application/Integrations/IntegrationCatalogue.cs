namespace Sencecon.Application.Integrations;

// The fixed set of integrations this app knows about. There's no "create a new
// integration" concept (same pragmatic-fixed-list spirit as the 5 user roles) —
// keys/names mirror the frontend's mock catalogue in mockData.js exactly, so
// existing and future rows always resolve to one of these.
public static class IntegrationCatalogue
{
    public static readonly IReadOnlyList<(string Key, string Name)> Entries =
    [
        ("sms", "SMS gateway"),
        ("whatsapp", "WhatsApp Business API"),
        ("weather", "Weather data feed"),
        ("zesco", "ZESCO tariff feed"),
    ];

    public static bool IsKnown(string key) => Entries.Any(e => e.Key == key);

    public static string? NameFor(string key) => Entries.FirstOrDefault(e => e.Key == key).Name;
}

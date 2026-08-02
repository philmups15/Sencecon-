using Sencecon.Domain.Common;

namespace Sencecon.Domain.Entities;

// Option A from docs/admin-redesign-brief.md: single global row per integration,
// no tenant scoping — this backend has no Tenant entity at all yet (the frontend
// tenant switcher is mock-only), so per-tenant settings would need that built
// first. See docs/admin-phase2-backend-brief.md.
public class IntegrationSetting : BaseAuditableEntity
{
    public string Key { get; set; } = string.Empty;
    public string? ProviderEndpoint { get; set; }

    // Encrypted via ISecretProtector (ASP.NET Data Protection) before storage —
    // never stored or returned in plaintext.
    public string? ApiKeyCipher { get; set; }

    public string? Notes { get; set; }
}

using MediatR;
using Microsoft.EntityFrameworkCore;
using Sencecon.Application.Common.Interfaces;

namespace Sencecon.Application.Integrations.Queries.GetIntegrationSettings;

public record GetIntegrationSettingsQuery : IRequest<IReadOnlyList<IntegrationSettingDto>>;

public class GetIntegrationSettingsQueryHandler : IRequestHandler<GetIntegrationSettingsQuery, IReadOnlyList<IntegrationSettingDto>>
{
    private readonly IApplicationDbContext _context;

    public GetIntegrationSettingsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<IntegrationSettingDto>> Handle(GetIntegrationSettingsQuery request, CancellationToken cancellationToken)
    {
        var rows = await _context.IntegrationSettings.ToListAsync(cancellationToken);
        var byKey = rows.ToDictionary(r => r.Key);

        // Always returns one entry per catalogue item, even if no row has been
        // saved for it yet — a fixed 4-item list, not an open-ended table.
        return IntegrationCatalogue.Entries
            .Select(entry =>
            {
                byKey.TryGetValue(entry.Key, out var row);
                var isConfigured = row is not null && (!string.IsNullOrEmpty(row.ProviderEndpoint) || !string.IsNullOrEmpty(row.ApiKeyCipher));

                return new IntegrationSettingDto
                {
                    Key = entry.Key,
                    Name = entry.Name,
                    Status = isConfigured ? "Connected" : "Not configured",
                    ProviderEndpoint = row?.ProviderEndpoint,
                    HasApiKey = !string.IsNullOrEmpty(row?.ApiKeyCipher),
                    Notes = row?.Notes,
                    LastModified = row?.LastModified
                };
            })
            .ToList();
    }
}

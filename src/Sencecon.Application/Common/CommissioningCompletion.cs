using Microsoft.EntityFrameworkCore;
using Sencecon.Application.Common.Interfaces;
using Sencecon.Domain.Enums;

namespace Sencecon.Application.Common;

// Shared check used by the handover sign-off and the Plant -> Operating gate:
// every applicable commissioning test for the plant must have a Pass result.
public static class CommissioningCompletion
{
    public static async Task<(bool Complete, IReadOnlyList<string> Missing)> EvaluateAsync(
        IApplicationDbContext context, Guid plantId, PlantType plantType, CancellationToken cancellationToken)
    {
        var pt = (int)plantType;

        var templates = await context.CommissioningTestTemplates
            .Where(t => t.IsActive)
            .ToListAsync(cancellationToken);

        var applicable = templates
            .Where(t => t.AppliesToTypes.Length == 0 || t.AppliesToTypes.Contains(pt))
            .ToList();

        var results = await context.CommissioningTestResults
            .Where(r => r.PlantId == plantId)
            .ToListAsync(cancellationToken);

        var missing = new List<string>();
        foreach (var t in applicable)
        {
            var match = results.FirstOrDefault(r => r.Category == t.Category && r.TestName == t.TestName);
            if (match is null)
                missing.Add($"{t.TestName} (not recorded)");
            else if (match.Result != CommissioningResultStatus.Pass)
                missing.Add($"{t.TestName} ({match.Result})");
        }

        return (missing.Count == 0, missing);
    }
}

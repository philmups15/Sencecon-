using Microsoft.EntityFrameworkCore;

namespace Sencecon.Application.Common;

public static class EntityCodeGenerator
{
    public static async Task<string> GenerateNextCodeAsync(IQueryable<string> codeQuery, string prefix, CancellationToken cancellationToken)
    {
        var codes = await codeQuery.ToListAsync(cancellationToken);

        var nextNumber = codes
            .Select(c => c.StartsWith(prefix) && int.TryParse(c.AsSpan(prefix.Length), out var n) ? n : 0)
            .DefaultIfEmpty(0)
            .Max() + 1;

        return $"{prefix}{nextNumber:000}";
    }
}

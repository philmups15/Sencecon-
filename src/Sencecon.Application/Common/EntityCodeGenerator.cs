using Sencecon.Application.Common.Interfaces;

namespace Sencecon.Application.Common;

// Human-readable entity codes (PLT-001, WO-042, ...). Backed by a dedicated
// Postgres sequence per prefix (created in migration AddCodeSequences), so
// concurrent creates get distinct numbers without a read-modify-write race.
public static class EntityCodeGenerator
{
    private static string SequenceFor(string prefix) => prefix.TrimEnd('-').ToLowerInvariant() + "_code_seq";

    public static async Task<string> GenerateNextCodeAsync(
        IApplicationDbContext context, string prefix, CancellationToken cancellationToken)
    {
        var next = await context.NextSequenceValueAsync(SequenceFor(prefix), cancellationToken);
        return $"{prefix}{next:000}";
    }
}

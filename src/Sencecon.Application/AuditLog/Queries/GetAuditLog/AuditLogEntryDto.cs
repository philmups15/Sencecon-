namespace Sencecon.Application.AuditLog.Queries.GetAuditLog;

public record AuditLogEntryDto
{
    public Guid Id { get; init; }
    public string Who { get; init; } = string.Empty;
    public string Action { get; init; } = string.Empty;
    public DateTimeOffset Created { get; init; }
}

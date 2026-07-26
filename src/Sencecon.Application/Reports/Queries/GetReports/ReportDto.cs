namespace Sencecon.Application.Reports.Queries.GetReports;

public record ReportDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string GeneratedBy { get; init; } = string.Empty;
    public DateTimeOffset GeneratedDate { get; init; }
}

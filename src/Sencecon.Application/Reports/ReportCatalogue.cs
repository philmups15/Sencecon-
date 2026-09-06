using Sencecon.Domain.Enums;

namespace Sencecon.Application.Reports;

// The fixed set of report types a user can generate. Generation currently
// records a metadata row; the PDF/spreadsheet itself is produced client-side.
public static class ReportCatalogue
{
    public record Entry(ReportType Type, string Name, string Description);

    public static readonly IReadOnlyList<Entry> Entries =
    [
        new(ReportType.MonthlyPerformance, "Monthly performance summary", "PR, yield and availability per plant"),
        new(ReportType.SlaCompliance, "SLA compliance report", "Work-order response and resolution times"),
        new(ReportType.PortfolioHealth, "Portfolio health report", "Cross-fleet health and attention flags"),
        new(ReportType.HandoverAudit, "Handover audit report", "Commissioning artefacts by project"),
    ];

    public static Entry For(ReportType type) => Entries.First(e => e.Type == type);
}

using MediatR;
using Sencecon.Domain.Enums;

namespace Sencecon.Application.Reports.Queries.GetReportCatalogue;

public record ReportCatalogueEntryDto
{
    public ReportType Type { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
}

public record GetReportCatalogueQuery : IRequest<IReadOnlyList<ReportCatalogueEntryDto>>;

public class GetReportCatalogueQueryHandler : IRequestHandler<GetReportCatalogueQuery, IReadOnlyList<ReportCatalogueEntryDto>>
{
    public Task<IReadOnlyList<ReportCatalogueEntryDto>> Handle(GetReportCatalogueQuery request, CancellationToken cancellationToken)
    {
        IReadOnlyList<ReportCatalogueEntryDto> entries = ReportCatalogue.Entries
            .Select(e => new ReportCatalogueEntryDto { Type = e.Type, Name = e.Name, Description = e.Description })
            .ToList();

        return Task.FromResult(entries);
    }
}

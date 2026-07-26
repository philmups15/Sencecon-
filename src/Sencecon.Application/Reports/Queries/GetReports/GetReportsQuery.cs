using MediatR;
using Microsoft.EntityFrameworkCore;
using Sencecon.Application.Common.Interfaces;

namespace Sencecon.Application.Reports.Queries.GetReports;

public record GetReportsQuery : IRequest<IReadOnlyList<ReportDto>>;

public class GetReportsQueryHandler : IRequestHandler<GetReportsQuery, IReadOnlyList<ReportDto>>
{
    private readonly IApplicationDbContext _context;

    public GetReportsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<ReportDto>> Handle(GetReportsQuery request, CancellationToken cancellationToken)
    {
        return await _context.Reports
            .OrderByDescending(r => r.GeneratedDate)
            .Select(r => new ReportDto
            {
                Id = r.Id,
                Name = r.Name,
                GeneratedBy = r.GeneratedBy,
                GeneratedDate = r.GeneratedDate
            })
            .ToListAsync(cancellationToken);
    }
}

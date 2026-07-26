using MediatR;
using Microsoft.EntityFrameworkCore;
using Sencecon.Application.Common.Interfaces;

namespace Sencecon.Application.AuditLog.Queries.GetAuditLog;

public record GetAuditLogQuery : IRequest<IReadOnlyList<AuditLogEntryDto>>;

public class GetAuditLogQueryHandler : IRequestHandler<GetAuditLogQuery, IReadOnlyList<AuditLogEntryDto>>
{
    private readonly IApplicationDbContext _context;

    public GetAuditLogQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<AuditLogEntryDto>> Handle(GetAuditLogQuery request, CancellationToken cancellationToken)
    {
        return await _context.AuditLogEntries
            .OrderByDescending(a => a.Created)
            .Take(100)
            .Select(a => new AuditLogEntryDto
            {
                Id = a.Id,
                Who = a.User != null ? a.User.DisplayName : "System",
                Action = a.Action,
                Created = a.Created
            })
            .ToListAsync(cancellationToken);
    }
}

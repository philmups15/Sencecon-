using MediatR;
using Sencecon.Application.Common.Interfaces;
using Sencecon.Domain.Entities;
using Sencecon.Domain.Enums;

namespace Sencecon.Application.Reports.Commands.CreateReport;

public record CreateReportCommand : IRequest<Guid>
{
    public required ReportType Type { get; init; }
    public string GeneratedBy { get; init; } = string.Empty;
}

public class CreateReportCommandHandler : IRequestHandler<CreateReportCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public CreateReportCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreateReportCommand request, CancellationToken cancellationToken)
    {
        var catalogue = ReportCatalogue.For(request.Type);
        var now = DateTimeOffset.UtcNow;

        var entity = new Report
        {
            Name = $"{catalogue.Name} — {now:MMM yyyy}",
            Type = request.Type,
            GeneratedBy = request.GeneratedBy,
            GeneratedDate = now,
            Created = now
        };

        _context.Reports.Add(entity);

        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}

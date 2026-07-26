using MediatR;
using Sencecon.Application.Common.Interfaces;
using Sencecon.Domain.Entities;

namespace Sencecon.Application.Reports.Commands.CreateReport;

public record CreateReportCommand : IRequest<Guid>
{
    public required string Name { get; init; }
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
        var entity = new Report
        {
            Name = request.Name,
            GeneratedBy = request.GeneratedBy,
            GeneratedDate = DateTimeOffset.UtcNow,
            Created = DateTimeOffset.UtcNow
        };

        _context.Reports.Add(entity);

        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}

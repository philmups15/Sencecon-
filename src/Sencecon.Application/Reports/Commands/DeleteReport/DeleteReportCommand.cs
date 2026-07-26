using MediatR;
using Microsoft.EntityFrameworkCore;
using Sencecon.Application.Common.Interfaces;
using Sencecon.Domain.Exceptions;

namespace Sencecon.Application.Reports.Commands.DeleteReport;

public record DeleteReportCommand : IRequest
{
    public required Guid Id { get; init; }
}

public class DeleteReportCommandHandler : IRequestHandler<DeleteReportCommand>
{
    private readonly IApplicationDbContext _context;

    public DeleteReportCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(DeleteReportCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Reports
            .FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken);

        if (entity is null)
        {
            throw new NotFoundException(nameof(Domain.Entities.Report), request.Id);
        }

        _context.Reports.Remove(entity);

        await _context.SaveChangesAsync(cancellationToken);
    }
}

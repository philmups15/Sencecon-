using MediatR;
using Microsoft.EntityFrameworkCore;
using Sencecon.Application.Common.Interfaces;
using Sencecon.Domain.Exceptions;

namespace Sencecon.Application.Surveys.Commands.DeleteSurvey;

public record DeleteSurveyCommand : IRequest
{
    public required Guid Id { get; init; }
}

public class DeleteSurveyCommandHandler : IRequestHandler<DeleteSurveyCommand>
{
    private readonly IApplicationDbContext _context;

    public DeleteSurveyCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(DeleteSurveyCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Surveys
            .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);

        if (entity is null)
        {
            throw new NotFoundException(nameof(Domain.Entities.Survey), request.Id);
        }

        _context.Surveys.Remove(entity);

        await _context.SaveChangesAsync(cancellationToken);
    }
}

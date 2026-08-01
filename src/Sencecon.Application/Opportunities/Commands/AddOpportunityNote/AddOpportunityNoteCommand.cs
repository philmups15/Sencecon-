using MediatR;
using Microsoft.EntityFrameworkCore;
using Sencecon.Application.Common.Interfaces;
using Sencecon.Domain.Entities;
using Sencecon.Domain.Exceptions;

namespace Sencecon.Application.Opportunities.Commands.AddOpportunityNote;

public record AddOpportunityNoteCommand : IRequest<Guid>
{
    public required Guid OpportunityId { get; init; }
    public required string Text { get; init; }
}

public class AddOpportunityNoteCommandHandler : IRequestHandler<AddOpportunityNoteCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public AddOpportunityNoteCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Guid> Handle(AddOpportunityNoteCommand request, CancellationToken cancellationToken)
    {
        var opportunityExists = await _context.Opportunities
            .AnyAsync(o => o.Id == request.OpportunityId, cancellationToken);

        if (!opportunityExists)
        {
            throw new NotFoundException(nameof(Domain.Entities.Opportunity), request.OpportunityId);
        }

        var currentUserId = _currentUserService.UserId
            ?? throw new UnauthorizedAccessException("No authenticated user.");

        var note = new OpportunityNote
        {
            OpportunityId = request.OpportunityId,
            Text = request.Text,
            CreatedBy = currentUserId,
            Created = DateTimeOffset.UtcNow
        };

        _context.OpportunityNotes.Add(note);

        OpportunityActivityLogger.Log(_context, request.OpportunityId, "note", $"Added note: \"{request.Text}\"", currentUserId);

        await _context.SaveChangesAsync(cancellationToken);

        return note.Id;
    }
}

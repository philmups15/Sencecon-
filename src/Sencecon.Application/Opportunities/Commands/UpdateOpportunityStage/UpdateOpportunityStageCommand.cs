using MediatR;
using Microsoft.EntityFrameworkCore;
using Sencecon.Application.Common.Interfaces;
using Sencecon.Domain.Enums;
using Sencecon.Domain.Exceptions;

namespace Sencecon.Application.Opportunities.Commands.UpdateOpportunityStage;

public record UpdateOpportunityStageCommand : IRequest
{
    public required Guid Id { get; init; }
    public required OpportunityStage Stage { get; init; }
    public string? NextAction { get; init; }
    public DateTimeOffset? SiteVisitDate { get; init; }
    public string? SiteVisitNotes { get; init; }
    public string? ProposalNotes { get; init; }
    public string? NegotiationNotes { get; init; }
    public DateTimeOffset? WonDate { get; init; }
}

public class UpdateOpportunityStageCommandHandler : IRequestHandler<UpdateOpportunityStageCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateOpportunityStageCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(UpdateOpportunityStageCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Opportunities
            .FirstOrDefaultAsync(o => o.Id == request.Id, cancellationToken);

        if (entity is null)
        {
            throw new NotFoundException(nameof(Domain.Entities.Opportunity), request.Id);
        }

        entity.Stage = request.Stage;

        if (request.NextAction is not null)
        {
            entity.NextAction = request.NextAction;
        }

        switch (request.Stage)
        {
            case OpportunityStage.SiteVisit:
                entity.SiteVisitDate = request.SiteVisitDate ?? entity.SiteVisitDate ?? DateTimeOffset.UtcNow;
                entity.SiteVisitNotes = request.SiteVisitNotes ?? entity.SiteVisitNotes;
                break;
            case OpportunityStage.Proposal:
                entity.ProposalNotes = request.ProposalNotes ?? entity.ProposalNotes;
                break;
            case OpportunityStage.Negotiation:
                entity.NegotiationNotes = request.NegotiationNotes ?? entity.NegotiationNotes;
                break;
            case OpportunityStage.Won:
                entity.WonDate = request.WonDate ?? entity.WonDate ?? DateTimeOffset.UtcNow;
                break;
        }

        entity.LastModified = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
    }
}

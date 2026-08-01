using MediatR;
using Microsoft.EntityFrameworkCore;
using Sencecon.Application.Common.Interfaces;
using Sencecon.Domain.Entities;
using Sencecon.Domain.Enums;
using Sencecon.Domain.Exceptions;

namespace Sencecon.Application.Opportunities.Commands.ChangeOpportunityStage;

public record ChangeOpportunityStageCommand : IRequest
{
    public required Guid OpportunityId { get; init; }
    public required OpportunityStage Stage { get; init; }
    public string? Note { get; init; }
}

public class ChangeOpportunityStageCommandHandler : IRequestHandler<ChangeOpportunityStageCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public ChangeOpportunityStageCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task Handle(ChangeOpportunityStageCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Opportunities
            .FirstOrDefaultAsync(o => o.Id == request.OpportunityId, cancellationToken);

        if (entity is null)
        {
            throw new NotFoundException(nameof(Domain.Entities.Opportunity), request.OpportunityId);
        }

        if (entity.Stage == request.Stage)
        {
            return;
        }

        var currentUserId = _currentUserService.UserId;
        var from = OpportunityActivityLogger.StageLabel(entity.Stage);
        var to = OpportunityActivityLogger.StageLabel(request.Stage);
        var note = request.Note?.Trim();

        var text = string.IsNullOrEmpty(note)
            ? $"Moved from {from} to {to}"
            : $"Moved from {from} to {to} — \"{note}\"";

        OpportunityActivityLogger.Log(_context, entity.Id, "stage", text, currentUserId);

        if (!string.IsNullOrEmpty(note))
        {
            _context.OpportunityNotes.Add(new OpportunityNote
            {
                OpportunityId = entity.Id,
                Text = note,
                CreatedBy = currentUserId ?? Guid.Empty,
                Created = DateTimeOffset.UtcNow
            });
        }

        entity.Stage = request.Stage;

        var stageKey = request.Stage.ToString();
        if (!entity.StageData.ContainsKey(stageKey))
        {
            var updated = new Dictionary<string, Dictionary<string, string>>(entity.StageData)
            {
                [stageKey] = new Dictionary<string, string>()
            };
            entity.StageData = updated;
        }

        entity.LastModified = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
    }
}

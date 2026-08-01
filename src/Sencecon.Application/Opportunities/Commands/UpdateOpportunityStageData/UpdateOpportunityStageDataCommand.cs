using MediatR;
using Microsoft.EntityFrameworkCore;
using Sencecon.Application.Common.Interfaces;
using Sencecon.Domain.Exceptions;

namespace Sencecon.Application.Opportunities.Commands.UpdateOpportunityStageData;

public record UpdateOpportunityStageDataCommand : IRequest
{
    public required Guid OpportunityId { get; init; }
    public required Dictionary<string, string> Fields { get; init; }
}

public class UpdateOpportunityStageDataCommandHandler : IRequestHandler<UpdateOpportunityStageDataCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public UpdateOpportunityStageDataCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task Handle(UpdateOpportunityStageDataCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Opportunities
            .FirstOrDefaultAsync(o => o.Id == request.OpportunityId, cancellationToken);

        if (entity is null)
        {
            throw new NotFoundException(nameof(Domain.Entities.Opportunity), request.OpportunityId);
        }

        var currentUserId = _currentUserService.UserId;
        var stageKey = entity.Stage.ToString();
        var stageLabel = OpportunityActivityLogger.StageLabel(entity.Stage);
        var existing = entity.StageData.TryGetValue(stageKey, out var current)
            ? new Dictionary<string, string>(current)
            : new Dictionary<string, string>();

        foreach (var (fieldKey, newValue) in request.Fields)
        {
            var oldValue = existing.TryGetValue(fieldKey, out var v) ? v : string.Empty;
            if (oldValue == newValue)
            {
                continue;
            }

            var fieldLabel = FieldLabel(fieldKey);
            OpportunityActivityLogger.Log(
                _context,
                entity.Id,
                "edit",
                $"{stageLabel} — {fieldLabel} set to \"{(string.IsNullOrEmpty(newValue) ? "—" : newValue)}\"",
                currentUserId);

            existing[fieldKey] = newValue;
        }

        var updatedStageData = new Dictionary<string, Dictionary<string, string>>(entity.StageData)
        {
            [stageKey] = existing
        };
        entity.StageData = updatedStageData;
        entity.LastModified = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
    }

    private static string FieldLabel(string fieldKey) => fieldKey switch
    {
        "leadSource" => "Lead source",
        "budgetConfirmed" => "Budget status",
        "decisionMaker" => "Decision maker",
        "visitDate" => "Visit date",
        "roofCondition" => "Site condition",
        "siteContact" => "Site contact",
        "proposalVersion" => "Proposal version",
        "systemSize" => "System size quoted",
        "quotedPrice" => "Quoted price",
        "discountPct" => "Discount offered",
        "expectedClose" => "Expected close date",
        "termsNotes" => "Terms notes",
        "contractDate" => "Contract signed date",
        "poNumber" => "PO number",
        _ => fieldKey
    };
}

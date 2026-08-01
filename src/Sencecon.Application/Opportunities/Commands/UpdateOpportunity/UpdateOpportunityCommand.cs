using MediatR;
using Microsoft.EntityFrameworkCore;
using Sencecon.Application.Common.Interfaces;
using Sencecon.Domain.Enums;
using Sencecon.Domain.Exceptions;

namespace Sencecon.Application.Opportunities.Commands.UpdateOpportunity;

public record UpdateOpportunityCommand : IRequest
{
    public required Guid Id { get; init; }
    public required string Code { get; init; }
    public required string Customer { get; init; }
    public string Capacity { get; init; } = string.Empty;
    public OpportunityStage Stage { get; init; }
    public string Location { get; init; } = string.Empty;
    public string NextAction { get; init; } = string.Empty;
    public string Owner { get; init; } = string.Empty;
    public decimal Value { get; init; }
}

public class UpdateOpportunityCommandHandler : IRequestHandler<UpdateOpportunityCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public UpdateOpportunityCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task Handle(UpdateOpportunityCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Opportunities
            .FirstOrDefaultAsync(o => o.Id == request.Id, cancellationToken);

        if (entity is null)
        {
            throw new NotFoundException(nameof(Domain.Entities.Opportunity), request.Id);
        }

        var currentUserId = _currentUserService.UserId;

        LogFieldChange(entity.Id, "Customer", entity.Customer, request.Customer, currentUserId);
        LogFieldChange(entity.Id, "Capacity", entity.Capacity, request.Capacity, currentUserId);
        LogFieldChange(entity.Id, "Location", entity.Location, request.Location, currentUserId);
        LogFieldChange(entity.Id, "Owner", entity.Owner, request.Owner, currentUserId);
        LogFieldChange(entity.Id, "Next action", entity.NextAction, request.NextAction, currentUserId);

        if (entity.Value != request.Value)
        {
            OpportunityActivityLogger.Log(
                _context,
                entity.Id,
                "edit",
                $"Value changed from {OpportunityActivityLogger.FormatMoney(entity.Value)} to {OpportunityActivityLogger.FormatMoney(request.Value)}",
                currentUserId);
        }

        entity.Code = request.Code;
        entity.Customer = request.Customer;
        entity.Capacity = request.Capacity;
        entity.Stage = request.Stage;
        entity.Location = request.Location;
        entity.NextAction = request.NextAction;
        entity.Owner = request.Owner;
        entity.Value = request.Value;
        entity.LastModified = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
    }

    private void LogFieldChange(Guid opportunityId, string label, string oldValue, string newValue, Guid? userId)
    {
        if (oldValue == newValue)
        {
            return;
        }

        OpportunityActivityLogger.Log(
            _context,
            opportunityId,
            "edit",
            $"{label} changed from \"{(string.IsNullOrEmpty(oldValue) ? "—" : oldValue)}\" to \"{(string.IsNullOrEmpty(newValue) ? "—" : newValue)}\"",
            userId);
    }
}

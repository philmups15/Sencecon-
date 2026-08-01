using MediatR;
using Microsoft.EntityFrameworkCore;
using Sencecon.Application.Common.Interfaces;
using Sencecon.Domain.Entities;
using Sencecon.Domain.Enums;

namespace Sencecon.Application.Opportunities.Commands.CreateOpportunity;

public record CreateOpportunityCommand : IRequest<Guid>
{
    public required string Customer { get; init; }
    public string Capacity { get; init; } = string.Empty;
    public OpportunityStage Stage { get; init; } = OpportunityStage.Qualifying;
    public string Location { get; init; } = string.Empty;
    public string NextAction { get; init; } = string.Empty;
    public string Owner { get; init; } = string.Empty;
    public decimal Value { get; init; }
}

public class CreateOpportunityCommandHandler : IRequestHandler<CreateOpportunityCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public CreateOpportunityCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Guid> Handle(CreateOpportunityCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId
            ?? throw new UnauthorizedAccessException("No authenticated user.");

        var code = await GenerateNextCodeAsync(cancellationToken);

        var entity = new Opportunity
        {
            Code = code,
            Customer = request.Customer,
            Capacity = request.Capacity,
            Stage = request.Stage,
            Location = request.Location,
            NextAction = request.NextAction,
            Owner = request.Owner,
            Value = request.Value,
            CreatedBy = currentUserId,
            Created = DateTimeOffset.UtcNow,
            StageData = new() { [request.Stage.ToString()] = new() }
        };

        _context.Opportunities.Add(entity);

        OpportunityActivityLogger.Log(_context, entity.Id, "created", "Created opportunity", currentUserId);

        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }

    private async Task<string> GenerateNextCodeAsync(CancellationToken cancellationToken)
    {
        const string prefix = "OPP-";

        var codes = await _context.Opportunities
            .Select(o => o.Code)
            .ToListAsync(cancellationToken);

        var nextNumber = codes
            .Select(c => c.StartsWith(prefix) && int.TryParse(c.AsSpan(prefix.Length), out var n) ? n : 0)
            .DefaultIfEmpty(0)
            .Max() + 1;

        return $"{prefix}{nextNumber:000}";
    }
}

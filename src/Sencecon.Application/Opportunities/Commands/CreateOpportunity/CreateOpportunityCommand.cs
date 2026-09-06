using MediatR;
using Sencecon.Application.Common;
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

        var code = await EntityCodeGenerator.GenerateNextCodeAsync(_context, "OPP-", cancellationToken);

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
}

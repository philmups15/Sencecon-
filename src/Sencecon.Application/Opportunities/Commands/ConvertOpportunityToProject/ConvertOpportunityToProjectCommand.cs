using MediatR;
using Microsoft.EntityFrameworkCore;
using Sencecon.Application.Common.Interfaces;
using Sencecon.Domain.Enums;
using Sencecon.Domain.Exceptions;

namespace Sencecon.Application.Opportunities.Commands.ConvertOpportunityToProject;

public record ConvertOpportunityToProjectCommand : IRequest
{
    public required Guid OpportunityId { get; init; }
}

public class ConvertOpportunityToProjectCommandHandler : IRequestHandler<ConvertOpportunityToProjectCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public ConvertOpportunityToProjectCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task Handle(ConvertOpportunityToProjectCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Opportunities
            .FirstOrDefaultAsync(o => o.Id == request.OpportunityId, cancellationToken);

        if (entity is null)
        {
            throw new NotFoundException(nameof(Domain.Entities.Opportunity), request.OpportunityId);
        }

        if (entity.Stage != OpportunityStage.Won)
        {
            throw new ConflictException("Only opportunities in the Won stage can be converted to a project.");
        }

        if (entity.Converted)
        {
            throw new ConflictException("This opportunity has already been converted to a project.");
        }

        entity.Converted = true;
        entity.LastModified = DateTimeOffset.UtcNow;

        OpportunityActivityLogger.Log(_context, entity.Id, "edit", "Converted opportunity to project", _currentUserService.UserId);

        await _context.SaveChangesAsync(cancellationToken);
    }
}

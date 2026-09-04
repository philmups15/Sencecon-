using MediatR;
using Microsoft.EntityFrameworkCore;
using Sencecon.Application.Common;
using Sencecon.Application.Common.Interfaces;
using Sencecon.Domain.Entities;
using Sencecon.Domain.Enums;
using Sencecon.Domain.Exceptions;

namespace Sencecon.Application.Opportunities.Commands.ConvertOpportunityToProject;

public record ConvertOpportunityToProjectCommand : IRequest<Guid>
{
    public required Guid OpportunityId { get; init; }
}

public class ConvertOpportunityToProjectCommandHandler : IRequestHandler<ConvertOpportunityToProjectCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public ConvertOpportunityToProjectCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Guid> Handle(ConvertOpportunityToProjectCommand request, CancellationToken cancellationToken)
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

        var code = await EntityCodeGenerator.GenerateNextCodeAsync(_context.Projects.Select(p => p.Code), "PRJ-", cancellationToken);

        var project = new Project
        {
            Code = code,
            Name = string.IsNullOrWhiteSpace(entity.Location) ? entity.Customer : $"{entity.Customer} – {entity.Location}",
            Customer = entity.Customer,
            Stage = LifecycleStage.DesignSurvey,
            ProjectManager = entity.Owner,
            Budget = entity.Value,
            Actual = 0,
            Created = DateTimeOffset.UtcNow
        };

        _context.Projects.Add(project);

        var plantCode = await EntityCodeGenerator.GenerateNextCodeAsync(_context.Plants.Select(p => p.Code), "PLT-", cancellationToken);

        var plant = new Plant
        {
            Code = plantCode,
            Name = project.Name,
            Stage = LifecycleStage.DesignSurvey,
            Capacity = entity.Capacity,
            Health = PlantHealth.Unknown,
            ProjectId = project.Id,
            Created = DateTimeOffset.UtcNow
        };

        _context.Plants.Add(plant);

        entity.Converted = true;
        entity.LastModified = DateTimeOffset.UtcNow;

        OpportunityActivityLogger.Log(_context, entity.Id, "edit", $"Converted opportunity to project {project.Code} and created plant {plant.Code}", _currentUserService.UserId);

        await _context.SaveChangesAsync(cancellationToken);

        return project.Id;
    }
}

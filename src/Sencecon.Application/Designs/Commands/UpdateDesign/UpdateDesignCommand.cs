using MediatR;
using Microsoft.EntityFrameworkCore;
using Sencecon.Application.Common.Interfaces;
using Sencecon.Domain.Entities;
using Sencecon.Domain.Enums;
using Sencecon.Domain.Exceptions;

namespace Sencecon.Application.Designs.Commands.UpdateDesign;

public record UpdateDesignCommand : IRequest
{
    public required Guid Id { get; init; }
    public required string ProjectName { get; init; }
    public DesignStatus Status { get; init; }
    public string Revision { get; init; } = string.Empty;
    public Guid? SurveyId { get; init; }
    public Guid? ProjectId { get; init; }
}

public class UpdateDesignCommandHandler : IRequestHandler<UpdateDesignCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public UpdateDesignCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task Handle(UpdateDesignCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Designs
            .FirstOrDefaultAsync(d => d.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Design), request.Id);

        if (request.SurveyId.HasValue && !await _context.Surveys.AnyAsync(s => s.Id == request.SurveyId.Value, cancellationToken))
            throw new NotFoundException(nameof(Domain.Entities.Survey), request.SurveyId.Value);

        if (request.ProjectId.HasValue && !await _context.Projects.AnyAsync(p => p.Id == request.ProjectId.Value, cancellationToken))
            throw new NotFoundException(nameof(Domain.Entities.Project), request.ProjectId.Value);

        var revisionChanged = !string.IsNullOrWhiteSpace(request.Revision)
            && !string.Equals(entity.Revision, request.Revision, StringComparison.OrdinalIgnoreCase);

        entity.ProjectName = request.ProjectName;
        entity.Status = request.Status;
        entity.Revision = request.Revision;
        entity.SurveyId = request.SurveyId;
        entity.ProjectId = request.ProjectId;
        entity.LastModified = DateTimeOffset.UtcNow;

        if (revisionChanged)
        {
            _context.DesignRevisions.Add(new DesignRevision
            {
                DesignId = entity.Id,
                Revision = request.Revision,
                Note = null,
                ChangedBy = _currentUserService.UserId,
                Created = DateTimeOffset.UtcNow
            });
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}

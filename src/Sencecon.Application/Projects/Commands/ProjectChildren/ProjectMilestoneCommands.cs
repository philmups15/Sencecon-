using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Sencecon.Application.Common.Interfaces;
using Sencecon.Domain.Entities;
using Sencecon.Domain.Enums;
using Sencecon.Domain.Exceptions;

namespace Sencecon.Application.Projects.Commands.ProjectChildren;

public record AddProjectMilestoneCommand : IRequest<Guid>
{
    public required Guid ProjectId { get; init; }
    public required string Label { get; init; }
    public MilestoneState State { get; init; }
    public int Order { get; init; }
}

public class AddProjectMilestoneCommandValidator : AbstractValidator<AddProjectMilestoneCommand>
{
    public AddProjectMilestoneCommandValidator()
    {
        RuleFor(v => v.ProjectId).NotEmpty();
        RuleFor(v => v.Label).NotEmpty().MaximumLength(200);
        RuleFor(v => v.State).IsInEnum();
    }
}

public class AddProjectMilestoneCommandHandler : IRequestHandler<AddProjectMilestoneCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    public AddProjectMilestoneCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<Guid> Handle(AddProjectMilestoneCommand request, CancellationToken cancellationToken)
    {
        if (!await _context.Projects.AnyAsync(p => p.Id == request.ProjectId, cancellationToken))
            throw new NotFoundException(nameof(Project), request.ProjectId);

        var entity = new ProjectMilestone
        {
            ProjectId = request.ProjectId,
            Label = request.Label,
            State = request.State,
            Order = request.Order,
            Created = DateTimeOffset.UtcNow
        };
        _context.ProjectMilestones.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }
}

public record UpdateProjectMilestoneCommand : IRequest
{
    public required Guid Id { get; init; }
    public required string Label { get; init; }
    public MilestoneState State { get; init; }
    public int Order { get; init; }
}

public class UpdateProjectMilestoneCommandValidator : AbstractValidator<UpdateProjectMilestoneCommand>
{
    public UpdateProjectMilestoneCommandValidator()
    {
        RuleFor(v => v.Id).NotEmpty();
        RuleFor(v => v.Label).NotEmpty().MaximumLength(200);
        RuleFor(v => v.State).IsInEnum();
    }
}

public class UpdateProjectMilestoneCommandHandler : IRequestHandler<UpdateProjectMilestoneCommand>
{
    private readonly IApplicationDbContext _context;
    public UpdateProjectMilestoneCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task Handle(UpdateProjectMilestoneCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.ProjectMilestones.FirstOrDefaultAsync(m => m.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(ProjectMilestone), request.Id);

        entity.Label = request.Label;
        entity.State = request.State;
        entity.Order = request.Order;
        entity.LastModified = DateTimeOffset.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);
    }
}

public record DeleteProjectMilestoneCommand : IRequest
{
    public required Guid Id { get; init; }
}

public class DeleteProjectMilestoneCommandHandler : IRequestHandler<DeleteProjectMilestoneCommand>
{
    private readonly IApplicationDbContext _context;
    public DeleteProjectMilestoneCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task Handle(DeleteProjectMilestoneCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.ProjectMilestones.FirstOrDefaultAsync(m => m.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(ProjectMilestone), request.Id);
        _context.ProjectMilestones.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }
}

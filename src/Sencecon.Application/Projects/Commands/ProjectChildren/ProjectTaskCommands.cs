using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Sencecon.Application.Common.Interfaces;
using Sencecon.Domain.Entities;
using Sencecon.Domain.Enums;
using Sencecon.Domain.Exceptions;

namespace Sencecon.Application.Projects.Commands.ProjectChildren;

public record AddProjectTaskCommand : IRequest<Guid>
{
    public required Guid ProjectId { get; init; }
    public required string Name { get; init; }
    public string Owner { get; init; } = string.Empty;
    public DateTimeOffset? DueDate { get; init; }
    public ProjectTaskStatus Status { get; init; }
}

public class AddProjectTaskCommandValidator : AbstractValidator<AddProjectTaskCommand>
{
    public AddProjectTaskCommandValidator()
    {
        RuleFor(v => v.ProjectId).NotEmpty();
        RuleFor(v => v.Name).NotEmpty().MaximumLength(200);
        RuleFor(v => v.Owner).MaximumLength(150);
        RuleFor(v => v.Status).IsInEnum();
    }
}

public class AddProjectTaskCommandHandler : IRequestHandler<AddProjectTaskCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    public AddProjectTaskCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<Guid> Handle(AddProjectTaskCommand request, CancellationToken cancellationToken)
    {
        if (!await _context.Projects.AnyAsync(p => p.Id == request.ProjectId, cancellationToken))
            throw new NotFoundException(nameof(Project), request.ProjectId);

        var entity = new ProjectTask
        {
            ProjectId = request.ProjectId,
            Name = request.Name,
            Owner = request.Owner,
            DueDate = request.DueDate,
            Status = request.Status,
            Created = DateTimeOffset.UtcNow
        };
        _context.ProjectTasks.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }
}

public record UpdateProjectTaskCommand : IRequest
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public string Owner { get; init; } = string.Empty;
    public DateTimeOffset? DueDate { get; init; }
    public ProjectTaskStatus Status { get; init; }
}

public class UpdateProjectTaskCommandValidator : AbstractValidator<UpdateProjectTaskCommand>
{
    public UpdateProjectTaskCommandValidator()
    {
        RuleFor(v => v.Id).NotEmpty();
        RuleFor(v => v.Name).NotEmpty().MaximumLength(200);
        RuleFor(v => v.Owner).MaximumLength(150);
        RuleFor(v => v.Status).IsInEnum();
    }
}

public class UpdateProjectTaskCommandHandler : IRequestHandler<UpdateProjectTaskCommand>
{
    private readonly IApplicationDbContext _context;
    public UpdateProjectTaskCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task Handle(UpdateProjectTaskCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.ProjectTasks.FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(ProjectTask), request.Id);

        entity.Name = request.Name;
        entity.Owner = request.Owner;
        entity.DueDate = request.DueDate;
        entity.Status = request.Status;
        entity.LastModified = DateTimeOffset.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);
    }
}

public record DeleteProjectTaskCommand : IRequest
{
    public required Guid Id { get; init; }
}

public class DeleteProjectTaskCommandHandler : IRequestHandler<DeleteProjectTaskCommand>
{
    private readonly IApplicationDbContext _context;
    public DeleteProjectTaskCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task Handle(DeleteProjectTaskCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.ProjectTasks.FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(ProjectTask), request.Id);
        _context.ProjectTasks.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }
}

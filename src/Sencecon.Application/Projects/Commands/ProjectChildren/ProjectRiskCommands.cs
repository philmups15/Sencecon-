using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Sencecon.Application.Common.Interfaces;
using Sencecon.Domain.Entities;
using Sencecon.Domain.Enums;
using Sencecon.Domain.Exceptions;

namespace Sencecon.Application.Projects.Commands.ProjectChildren;

public record AddProjectRiskCommand : IRequest<Guid>
{
    public required Guid ProjectId { get; init; }
    public required string Description { get; init; }
    public RiskSeverity Severity { get; init; }
    public string Mitigation { get; init; } = string.Empty;
    public RiskStatus Status { get; init; }
}

public class AddProjectRiskCommandValidator : AbstractValidator<AddProjectRiskCommand>
{
    public AddProjectRiskCommandValidator()
    {
        RuleFor(v => v.ProjectId).NotEmpty();
        RuleFor(v => v.Description).NotEmpty().MaximumLength(500);
        RuleFor(v => v.Mitigation).MaximumLength(500);
        RuleFor(v => v.Severity).IsInEnum();
        RuleFor(v => v.Status).IsInEnum();
    }
}

public class AddProjectRiskCommandHandler : IRequestHandler<AddProjectRiskCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    public AddProjectRiskCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<Guid> Handle(AddProjectRiskCommand request, CancellationToken cancellationToken)
    {
        if (!await _context.Projects.AnyAsync(p => p.Id == request.ProjectId, cancellationToken))
            throw new NotFoundException(nameof(Project), request.ProjectId);

        var entity = new ProjectRisk
        {
            ProjectId = request.ProjectId,
            Description = request.Description,
            Severity = request.Severity,
            Mitigation = request.Mitigation,
            Status = request.Status,
            Created = DateTimeOffset.UtcNow
        };
        _context.ProjectRisks.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }
}

public record UpdateProjectRiskCommand : IRequest
{
    public required Guid Id { get; init; }
    public required string Description { get; init; }
    public RiskSeverity Severity { get; init; }
    public string Mitigation { get; init; } = string.Empty;
    public RiskStatus Status { get; init; }
}

public class UpdateProjectRiskCommandValidator : AbstractValidator<UpdateProjectRiskCommand>
{
    public UpdateProjectRiskCommandValidator()
    {
        RuleFor(v => v.Id).NotEmpty();
        RuleFor(v => v.Description).NotEmpty().MaximumLength(500);
        RuleFor(v => v.Mitigation).MaximumLength(500);
        RuleFor(v => v.Severity).IsInEnum();
        RuleFor(v => v.Status).IsInEnum();
    }
}

public class UpdateProjectRiskCommandHandler : IRequestHandler<UpdateProjectRiskCommand>
{
    private readonly IApplicationDbContext _context;
    public UpdateProjectRiskCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task Handle(UpdateProjectRiskCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.ProjectRisks.FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(ProjectRisk), request.Id);

        entity.Description = request.Description;
        entity.Severity = request.Severity;
        entity.Mitigation = request.Mitigation;
        entity.Status = request.Status;
        entity.LastModified = DateTimeOffset.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);
    }
}

public record DeleteProjectRiskCommand : IRequest
{
    public required Guid Id { get; init; }
}

public class DeleteProjectRiskCommandHandler : IRequestHandler<DeleteProjectRiskCommand>
{
    private readonly IApplicationDbContext _context;
    public DeleteProjectRiskCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task Handle(DeleteProjectRiskCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.ProjectRisks.FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(ProjectRisk), request.Id);
        _context.ProjectRisks.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }
}

using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Sencecon.Application.Common.Interfaces;
using Sencecon.Domain.Entities;
using Sencecon.Domain.Enums;
using Sencecon.Domain.Exceptions;

namespace Sencecon.Application.Projects.Commands.ProjectChildren;

public record AddSubcontractorCommand : IRequest<Guid>
{
    public required Guid ProjectId { get; init; }
    public required string Name { get; init; }
    public string Scope { get; init; } = string.Empty;
    public SubcontractorStatus Status { get; init; }
}

public class AddSubcontractorCommandValidator : AbstractValidator<AddSubcontractorCommand>
{
    public AddSubcontractorCommandValidator()
    {
        RuleFor(v => v.ProjectId).NotEmpty();
        RuleFor(v => v.Name).NotEmpty().MaximumLength(200);
        RuleFor(v => v.Scope).MaximumLength(300);
        RuleFor(v => v.Status).IsInEnum();
    }
}

public class AddSubcontractorCommandHandler : IRequestHandler<AddSubcontractorCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    public AddSubcontractorCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<Guid> Handle(AddSubcontractorCommand request, CancellationToken cancellationToken)
    {
        if (!await _context.Projects.AnyAsync(p => p.Id == request.ProjectId, cancellationToken))
            throw new NotFoundException(nameof(Project), request.ProjectId);

        var entity = new Subcontractor
        {
            ProjectId = request.ProjectId,
            Name = request.Name,
            Scope = request.Scope,
            Status = request.Status,
            Created = DateTimeOffset.UtcNow
        };
        _context.Subcontractors.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }
}

public record UpdateSubcontractorCommand : IRequest
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public string Scope { get; init; } = string.Empty;
    public SubcontractorStatus Status { get; init; }
}

public class UpdateSubcontractorCommandValidator : AbstractValidator<UpdateSubcontractorCommand>
{
    public UpdateSubcontractorCommandValidator()
    {
        RuleFor(v => v.Id).NotEmpty();
        RuleFor(v => v.Name).NotEmpty().MaximumLength(200);
        RuleFor(v => v.Scope).MaximumLength(300);
        RuleFor(v => v.Status).IsInEnum();
    }
}

public class UpdateSubcontractorCommandHandler : IRequestHandler<UpdateSubcontractorCommand>
{
    private readonly IApplicationDbContext _context;
    public UpdateSubcontractorCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task Handle(UpdateSubcontractorCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Subcontractors.FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Subcontractor), request.Id);

        entity.Name = request.Name;
        entity.Scope = request.Scope;
        entity.Status = request.Status;
        entity.LastModified = DateTimeOffset.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);
    }
}

public record DeleteSubcontractorCommand : IRequest
{
    public required Guid Id { get; init; }
}

public class DeleteSubcontractorCommandHandler : IRequestHandler<DeleteSubcontractorCommand>
{
    private readonly IApplicationDbContext _context;
    public DeleteSubcontractorCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task Handle(DeleteSubcontractorCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Subcontractors.FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Subcontractor), request.Id);
        _context.Subcontractors.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }
}

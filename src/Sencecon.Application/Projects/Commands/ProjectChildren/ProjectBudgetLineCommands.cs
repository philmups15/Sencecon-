using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Sencecon.Application.Common.Interfaces;
using Sencecon.Domain.Entities;
using Sencecon.Domain.Enums;
using Sencecon.Domain.Exceptions;

namespace Sencecon.Application.Projects.Commands.ProjectChildren;

public record AddProjectBudgetLineCommand : IRequest<Guid>
{
    public required Guid ProjectId { get; init; }
    public required string Label { get; init; }
    public decimal BudgetAmount { get; init; }
    public decimal ActualAmount { get; init; }
    public BomCategory? Category { get; init; }
}

public class AddProjectBudgetLineCommandValidator : AbstractValidator<AddProjectBudgetLineCommand>
{
    public AddProjectBudgetLineCommandValidator()
    {
        RuleFor(v => v.ProjectId).NotEmpty();
        RuleFor(v => v.Label).NotEmpty().MaximumLength(200);
        RuleFor(v => v.BudgetAmount).GreaterThanOrEqualTo(0);
        RuleFor(v => v.ActualAmount).GreaterThanOrEqualTo(0);
        RuleFor(v => v.Category).IsInEnum().When(v => v.Category.HasValue);
    }
}

public class AddProjectBudgetLineCommandHandler : IRequestHandler<AddProjectBudgetLineCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    public AddProjectBudgetLineCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<Guid> Handle(AddProjectBudgetLineCommand request, CancellationToken cancellationToken)
    {
        if (!await _context.Projects.AnyAsync(p => p.Id == request.ProjectId, cancellationToken))
            throw new NotFoundException(nameof(Project), request.ProjectId);

        var entity = new ProjectBudgetLine
        {
            ProjectId = request.ProjectId,
            Label = request.Label,
            BudgetAmount = request.BudgetAmount,
            ActualAmount = request.ActualAmount,
            Category = request.Category,
            Created = DateTimeOffset.UtcNow
        };
        _context.ProjectBudgetLines.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }
}

public record UpdateProjectBudgetLineCommand : IRequest
{
    public required Guid Id { get; init; }
    public required string Label { get; init; }
    public decimal BudgetAmount { get; init; }
    public decimal ActualAmount { get; init; }
    public BomCategory? Category { get; init; }
}

public class UpdateProjectBudgetLineCommandValidator : AbstractValidator<UpdateProjectBudgetLineCommand>
{
    public UpdateProjectBudgetLineCommandValidator()
    {
        RuleFor(v => v.Id).NotEmpty();
        RuleFor(v => v.Label).NotEmpty().MaximumLength(200);
        RuleFor(v => v.BudgetAmount).GreaterThanOrEqualTo(0);
        RuleFor(v => v.ActualAmount).GreaterThanOrEqualTo(0);
        RuleFor(v => v.Category).IsInEnum().When(v => v.Category.HasValue);
    }
}

public class UpdateProjectBudgetLineCommandHandler : IRequestHandler<UpdateProjectBudgetLineCommand>
{
    private readonly IApplicationDbContext _context;
    public UpdateProjectBudgetLineCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task Handle(UpdateProjectBudgetLineCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.ProjectBudgetLines.FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(ProjectBudgetLine), request.Id);

        entity.Label = request.Label;
        entity.BudgetAmount = request.BudgetAmount;
        entity.ActualAmount = request.ActualAmount;
        entity.Category = request.Category;
        entity.LastModified = DateTimeOffset.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);
    }
}

public record DeleteProjectBudgetLineCommand : IRequest
{
    public required Guid Id { get; init; }
}

public class DeleteProjectBudgetLineCommandHandler : IRequestHandler<DeleteProjectBudgetLineCommand>
{
    private readonly IApplicationDbContext _context;
    public DeleteProjectBudgetLineCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task Handle(DeleteProjectBudgetLineCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.ProjectBudgetLines.FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(ProjectBudgetLine), request.Id);
        _context.ProjectBudgetLines.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }
}

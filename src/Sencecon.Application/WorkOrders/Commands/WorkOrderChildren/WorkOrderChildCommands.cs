using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Sencecon.Application.Common.Interfaces;
using Sencecon.Domain.Entities;
using Sencecon.Domain.Exceptions;

namespace Sencecon.Application.WorkOrders.Commands.WorkOrderChildren;

// ---- Checklist ----
public record AddChecklistItemCommand : IRequest<Guid>
{
    public required Guid WorkOrderId { get; init; }
    public required string Text { get; init; }
    public int Order { get; init; }
}

public class AddChecklistItemCommandValidator : AbstractValidator<AddChecklistItemCommand>
{
    public AddChecklistItemCommandValidator()
    {
        RuleFor(v => v.WorkOrderId).NotEmpty();
        RuleFor(v => v.Text).NotEmpty().MaximumLength(300);
    }
}

public class AddChecklistItemCommandHandler : IRequestHandler<AddChecklistItemCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    public AddChecklistItemCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<Guid> Handle(AddChecklistItemCommand request, CancellationToken cancellationToken)
    {
        if (!await _context.WorkOrders.AnyAsync(w => w.Id == request.WorkOrderId, cancellationToken))
            throw new NotFoundException(nameof(WorkOrder), request.WorkOrderId);

        var entity = new WorkOrderChecklistItem { WorkOrderId = request.WorkOrderId, Text = request.Text, Order = request.Order };
        _context.WorkOrderChecklistItems.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }
}

public record UpdateChecklistItemCommand : IRequest
{
    public required Guid Id { get; init; }
    public required string Text { get; init; }
    public bool IsDone { get; init; }
    public int Order { get; init; }
}

public class UpdateChecklistItemCommandValidator : AbstractValidator<UpdateChecklistItemCommand>
{
    public UpdateChecklistItemCommandValidator()
    {
        RuleFor(v => v.Id).NotEmpty();
        RuleFor(v => v.Text).NotEmpty().MaximumLength(300);
    }
}

public class UpdateChecklistItemCommandHandler : IRequestHandler<UpdateChecklistItemCommand>
{
    private readonly IApplicationDbContext _context;
    public UpdateChecklistItemCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task Handle(UpdateChecklistItemCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.WorkOrderChecklistItems.FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(WorkOrderChecklistItem), request.Id);
        entity.Text = request.Text;
        entity.IsDone = request.IsDone;
        entity.Order = request.Order;
        await _context.SaveChangesAsync(cancellationToken);
    }
}

public record DeleteChecklistItemCommand : IRequest { public required Guid Id { get; init; } }

public class DeleteChecklistItemCommandHandler : IRequestHandler<DeleteChecklistItemCommand>
{
    private readonly IApplicationDbContext _context;
    public DeleteChecklistItemCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task Handle(DeleteChecklistItemCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.WorkOrderChecklistItems.FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(WorkOrderChecklistItem), request.Id);
        _context.WorkOrderChecklistItems.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }
}

// ---- Parts ----
public record AddWorkOrderPartCommand : IRequest<Guid>
{
    public required Guid WorkOrderId { get; init; }
    public required string PartName { get; init; }
    public int Quantity { get; init; }
    public string? Notes { get; init; }
}

public class AddWorkOrderPartCommandValidator : AbstractValidator<AddWorkOrderPartCommand>
{
    public AddWorkOrderPartCommandValidator()
    {
        RuleFor(v => v.WorkOrderId).NotEmpty();
        RuleFor(v => v.PartName).NotEmpty().MaximumLength(200);
        RuleFor(v => v.Quantity).GreaterThan(0);
        RuleFor(v => v.Notes).MaximumLength(300);
    }
}

public class AddWorkOrderPartCommandHandler : IRequestHandler<AddWorkOrderPartCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    public AddWorkOrderPartCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<Guid> Handle(AddWorkOrderPartCommand request, CancellationToken cancellationToken)
    {
        if (!await _context.WorkOrders.AnyAsync(w => w.Id == request.WorkOrderId, cancellationToken))
            throw new NotFoundException(nameof(WorkOrder), request.WorkOrderId);

        var entity = new WorkOrderPart { WorkOrderId = request.WorkOrderId, PartName = request.PartName, Quantity = request.Quantity, Notes = request.Notes };
        _context.WorkOrderParts.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }
}

public record UpdateWorkOrderPartCommand : IRequest
{
    public required Guid Id { get; init; }
    public required string PartName { get; init; }
    public int Quantity { get; init; }
    public string? Notes { get; init; }
}

public class UpdateWorkOrderPartCommandValidator : AbstractValidator<UpdateWorkOrderPartCommand>
{
    public UpdateWorkOrderPartCommandValidator()
    {
        RuleFor(v => v.Id).NotEmpty();
        RuleFor(v => v.PartName).NotEmpty().MaximumLength(200);
        RuleFor(v => v.Quantity).GreaterThan(0);
        RuleFor(v => v.Notes).MaximumLength(300);
    }
}

public class UpdateWorkOrderPartCommandHandler : IRequestHandler<UpdateWorkOrderPartCommand>
{
    private readonly IApplicationDbContext _context;
    public UpdateWorkOrderPartCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task Handle(UpdateWorkOrderPartCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.WorkOrderParts.FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(WorkOrderPart), request.Id);
        entity.PartName = request.PartName;
        entity.Quantity = request.Quantity;
        entity.Notes = request.Notes;
        await _context.SaveChangesAsync(cancellationToken);
    }
}

public record DeleteWorkOrderPartCommand : IRequest { public required Guid Id { get; init; } }

public class DeleteWorkOrderPartCommandHandler : IRequestHandler<DeleteWorkOrderPartCommand>
{
    private readonly IApplicationDbContext _context;
    public DeleteWorkOrderPartCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task Handle(DeleteWorkOrderPartCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.WorkOrderParts.FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(WorkOrderPart), request.Id);
        _context.WorkOrderParts.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }
}

// ---- Labour ----
public record AddWorkOrderLabourCommand : IRequest<Guid>
{
    public required Guid WorkOrderId { get; init; }
    public required string PersonName { get; init; }
    public decimal Hours { get; init; }
    public DateTimeOffset? WorkDate { get; init; }
    public string? Notes { get; init; }
}

public class AddWorkOrderLabourCommandValidator : AbstractValidator<AddWorkOrderLabourCommand>
{
    public AddWorkOrderLabourCommandValidator()
    {
        RuleFor(v => v.WorkOrderId).NotEmpty();
        RuleFor(v => v.PersonName).NotEmpty().MaximumLength(150);
        RuleFor(v => v.Hours).GreaterThan(0);
        RuleFor(v => v.Notes).MaximumLength(300);
    }
}

public class AddWorkOrderLabourCommandHandler : IRequestHandler<AddWorkOrderLabourCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    public AddWorkOrderLabourCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<Guid> Handle(AddWorkOrderLabourCommand request, CancellationToken cancellationToken)
    {
        if (!await _context.WorkOrders.AnyAsync(w => w.Id == request.WorkOrderId, cancellationToken))
            throw new NotFoundException(nameof(WorkOrder), request.WorkOrderId);

        var entity = new WorkOrderLabour { WorkOrderId = request.WorkOrderId, PersonName = request.PersonName, Hours = request.Hours, WorkDate = request.WorkDate, Notes = request.Notes };
        _context.WorkOrderLabour.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }
}

public record UpdateWorkOrderLabourCommand : IRequest
{
    public required Guid Id { get; init; }
    public required string PersonName { get; init; }
    public decimal Hours { get; init; }
    public DateTimeOffset? WorkDate { get; init; }
    public string? Notes { get; init; }
}

public class UpdateWorkOrderLabourCommandValidator : AbstractValidator<UpdateWorkOrderLabourCommand>
{
    public UpdateWorkOrderLabourCommandValidator()
    {
        RuleFor(v => v.Id).NotEmpty();
        RuleFor(v => v.PersonName).NotEmpty().MaximumLength(150);
        RuleFor(v => v.Hours).GreaterThan(0);
        RuleFor(v => v.Notes).MaximumLength(300);
    }
}

public class UpdateWorkOrderLabourCommandHandler : IRequestHandler<UpdateWorkOrderLabourCommand>
{
    private readonly IApplicationDbContext _context;
    public UpdateWorkOrderLabourCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task Handle(UpdateWorkOrderLabourCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.WorkOrderLabour.FirstOrDefaultAsync(l => l.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(WorkOrderLabour), request.Id);
        entity.PersonName = request.PersonName;
        entity.Hours = request.Hours;
        entity.WorkDate = request.WorkDate;
        entity.Notes = request.Notes;
        await _context.SaveChangesAsync(cancellationToken);
    }
}

public record DeleteWorkOrderLabourCommand : IRequest { public required Guid Id { get; init; } }

public class DeleteWorkOrderLabourCommandHandler : IRequestHandler<DeleteWorkOrderLabourCommand>
{
    private readonly IApplicationDbContext _context;
    public DeleteWorkOrderLabourCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task Handle(DeleteWorkOrderLabourCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.WorkOrderLabour.FirstOrDefaultAsync(l => l.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(WorkOrderLabour), request.Id);
        _context.WorkOrderLabour.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }
}

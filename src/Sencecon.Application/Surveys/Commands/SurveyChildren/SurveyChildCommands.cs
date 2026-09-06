using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Sencecon.Application.Common.Interfaces;
using Sencecon.Domain.Entities;
using Sencecon.Domain.Exceptions;

namespace Sencecon.Application.Surveys.Commands.SurveyChildren;

// ---- Measurements ----
public record AddSurveyMeasurementCommand : IRequest<Guid>
{
    public required Guid SurveyId { get; init; }
    public required string Field { get; init; }
    public required string Value { get; init; }
}

public class AddSurveyMeasurementCommandValidator : AbstractValidator<AddSurveyMeasurementCommand>
{
    public AddSurveyMeasurementCommandValidator()
    {
        RuleFor(v => v.SurveyId).NotEmpty();
        RuleFor(v => v.Field).NotEmpty().MaximumLength(200);
        RuleFor(v => v.Value).NotEmpty().MaximumLength(400);
    }
}

public class AddSurveyMeasurementCommandHandler : IRequestHandler<AddSurveyMeasurementCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    public AddSurveyMeasurementCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<Guid> Handle(AddSurveyMeasurementCommand request, CancellationToken cancellationToken)
    {
        if (!await _context.Surveys.AnyAsync(s => s.Id == request.SurveyId, cancellationToken))
            throw new NotFoundException(nameof(Survey), request.SurveyId);

        var entity = new SurveyMeasurement { SurveyId = request.SurveyId, Field = request.Field, Value = request.Value, Created = DateTimeOffset.UtcNow };
        _context.SurveyMeasurements.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }
}

public record UpdateSurveyMeasurementCommand : IRequest
{
    public required Guid Id { get; init; }
    public required string Field { get; init; }
    public required string Value { get; init; }
}

public class UpdateSurveyMeasurementCommandValidator : AbstractValidator<UpdateSurveyMeasurementCommand>
{
    public UpdateSurveyMeasurementCommandValidator()
    {
        RuleFor(v => v.Id).NotEmpty();
        RuleFor(v => v.Field).NotEmpty().MaximumLength(200);
        RuleFor(v => v.Value).NotEmpty().MaximumLength(400);
    }
}

public class UpdateSurveyMeasurementCommandHandler : IRequestHandler<UpdateSurveyMeasurementCommand>
{
    private readonly IApplicationDbContext _context;
    public UpdateSurveyMeasurementCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task Handle(UpdateSurveyMeasurementCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.SurveyMeasurements.FirstOrDefaultAsync(m => m.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(SurveyMeasurement), request.Id);
        entity.Field = request.Field;
        entity.Value = request.Value;
        entity.LastModified = DateTimeOffset.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);
    }
}

public record DeleteSurveyMeasurementCommand : IRequest { public required Guid Id { get; init; } }

public class DeleteSurveyMeasurementCommandHandler : IRequestHandler<DeleteSurveyMeasurementCommand>
{
    private readonly IApplicationDbContext _context;
    public DeleteSurveyMeasurementCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task Handle(DeleteSurveyMeasurementCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.SurveyMeasurements.FirstOrDefaultAsync(m => m.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(SurveyMeasurement), request.Id);
        _context.SurveyMeasurements.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }
}

// ---- Obstructions ----
public record AddSurveyObstructionCommand : IRequest<Guid>
{
    public required Guid SurveyId { get; init; }
    public required string Item { get; init; }
    public string Impact { get; init; } = string.Empty;
}

public class AddSurveyObstructionCommandValidator : AbstractValidator<AddSurveyObstructionCommand>
{
    public AddSurveyObstructionCommandValidator()
    {
        RuleFor(v => v.SurveyId).NotEmpty();
        RuleFor(v => v.Item).NotEmpty().MaximumLength(200);
        RuleFor(v => v.Impact).MaximumLength(400);
    }
}

public class AddSurveyObstructionCommandHandler : IRequestHandler<AddSurveyObstructionCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    public AddSurveyObstructionCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<Guid> Handle(AddSurveyObstructionCommand request, CancellationToken cancellationToken)
    {
        if (!await _context.Surveys.AnyAsync(s => s.Id == request.SurveyId, cancellationToken))
            throw new NotFoundException(nameof(Survey), request.SurveyId);

        var entity = new SurveyObstruction { SurveyId = request.SurveyId, Item = request.Item, Impact = request.Impact, Created = DateTimeOffset.UtcNow };
        _context.SurveyObstructions.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }
}

public record UpdateSurveyObstructionCommand : IRequest
{
    public required Guid Id { get; init; }
    public required string Item { get; init; }
    public string Impact { get; init; } = string.Empty;
}

public class UpdateSurveyObstructionCommandValidator : AbstractValidator<UpdateSurveyObstructionCommand>
{
    public UpdateSurveyObstructionCommandValidator()
    {
        RuleFor(v => v.Id).NotEmpty();
        RuleFor(v => v.Item).NotEmpty().MaximumLength(200);
        RuleFor(v => v.Impact).MaximumLength(400);
    }
}

public class UpdateSurveyObstructionCommandHandler : IRequestHandler<UpdateSurveyObstructionCommand>
{
    private readonly IApplicationDbContext _context;
    public UpdateSurveyObstructionCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task Handle(UpdateSurveyObstructionCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.SurveyObstructions.FirstOrDefaultAsync(o => o.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(SurveyObstruction), request.Id);
        entity.Item = request.Item;
        entity.Impact = request.Impact;
        entity.LastModified = DateTimeOffset.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);
    }
}

public record DeleteSurveyObstructionCommand : IRequest { public required Guid Id { get; init; } }

public class DeleteSurveyObstructionCommandHandler : IRequestHandler<DeleteSurveyObstructionCommand>
{
    private readonly IApplicationDbContext _context;
    public DeleteSurveyObstructionCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task Handle(DeleteSurveyObstructionCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.SurveyObstructions.FirstOrDefaultAsync(o => o.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(SurveyObstruction), request.Id);
        _context.SurveyObstructions.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }
}

using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Sencecon.Application.Common.Interfaces;
using Sencecon.Domain.Entities;
using Sencecon.Domain.Enums;
using Sencecon.Domain.Exceptions;

namespace Sencecon.Application.Commissioning;

public record CommissioningTemplateDto
{
    public Guid Id { get; init; }
    public CommissioningTestCategory Category { get; init; }
    public string TestName { get; init; } = string.Empty;
    public IReadOnlyList<PlantType> AppliesToTypes { get; init; } = Array.Empty<PlantType>();
    public int Order { get; init; }
    public bool IsActive { get; init; }
}

public record GetCommissioningTemplatesQuery : IRequest<IReadOnlyList<CommissioningTemplateDto>>;

public class GetCommissioningTemplatesQueryHandler : IRequestHandler<GetCommissioningTemplatesQuery, IReadOnlyList<CommissioningTemplateDto>>
{
    private readonly IApplicationDbContext _context;
    public GetCommissioningTemplatesQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<IReadOnlyList<CommissioningTemplateDto>> Handle(GetCommissioningTemplatesQuery request, CancellationToken cancellationToken)
    {
        var rows = await _context.CommissioningTestTemplates
            .OrderBy(t => t.Order).ThenBy(t => t.TestName)
            .ToListAsync(cancellationToken);

        return rows.Select(t => new CommissioningTemplateDto
        {
            Id = t.Id,
            Category = t.Category,
            TestName = t.TestName,
            AppliesToTypes = t.AppliesToTypes.Select(i => (PlantType)i).ToList(),
            Order = t.Order,
            IsActive = t.IsActive
        }).ToList();
    }
}

public record UpsertCommissioningTemplateCommand : IRequest<Guid>
{
    public Guid? Id { get; init; }
    public required CommissioningTestCategory Category { get; init; }
    public required string TestName { get; init; }
    public IReadOnlyList<PlantType> AppliesToTypes { get; init; } = Array.Empty<PlantType>();
    public int Order { get; init; }
    public bool IsActive { get; init; } = true;
}

public class UpsertCommissioningTemplateCommandValidator : AbstractValidator<UpsertCommissioningTemplateCommand>
{
    public UpsertCommissioningTemplateCommandValidator()
    {
        RuleFor(v => v.Category).IsInEnum();
        RuleFor(v => v.TestName).NotEmpty().MaximumLength(200);
    }
}

public class UpsertCommissioningTemplateCommandHandler : IRequestHandler<UpsertCommissioningTemplateCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    public UpsertCommissioningTemplateCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<Guid> Handle(UpsertCommissioningTemplateCommand request, CancellationToken cancellationToken)
    {
        var appliesTo = request.AppliesToTypes.Select(t => (int)t).ToArray();

        CommissioningTestTemplate entity;
        if (request.Id.HasValue)
        {
            entity = await _context.CommissioningTestTemplates.FirstOrDefaultAsync(t => t.Id == request.Id.Value, cancellationToken)
                ?? throw new NotFoundException(nameof(CommissioningTestTemplate), request.Id.Value);
            entity.Category = request.Category;
            entity.TestName = request.TestName;
            entity.AppliesToTypes = appliesTo;
            entity.Order = request.Order;
            entity.IsActive = request.IsActive;
            entity.LastModified = DateTimeOffset.UtcNow;
        }
        else
        {
            entity = new CommissioningTestTemplate
            {
                Category = request.Category,
                TestName = request.TestName,
                AppliesToTypes = appliesTo,
                Order = request.Order,
                IsActive = request.IsActive,
                Created = DateTimeOffset.UtcNow
            };
            _context.CommissioningTestTemplates.Add(entity);
        }

        await _context.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }
}

public record DeleteCommissioningTemplateCommand : IRequest { public required Guid Id { get; init; } }

public class DeleteCommissioningTemplateCommandHandler : IRequestHandler<DeleteCommissioningTemplateCommand>
{
    private readonly IApplicationDbContext _context;
    public DeleteCommissioningTemplateCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task Handle(DeleteCommissioningTemplateCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.CommissioningTestTemplates.FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(CommissioningTestTemplate), request.Id);
        _context.CommissioningTestTemplates.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }
}

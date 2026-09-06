using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Sencecon.Application.Common.Interfaces;
using Sencecon.Domain.Entities;
using Sencecon.Domain.Exceptions;

namespace Sencecon.Application.Designs.Commands.DesignRevisionSpecs;

// --- Add a manual revision note ---
public record AddDesignRevisionCommand : IRequest<Guid>
{
    public required Guid DesignId { get; init; }
    public required string Revision { get; init; }
    public string? Note { get; init; }
}

public class AddDesignRevisionCommandValidator : AbstractValidator<AddDesignRevisionCommand>
{
    public AddDesignRevisionCommandValidator()
    {
        RuleFor(v => v.DesignId).NotEmpty();
        RuleFor(v => v.Revision).NotEmpty().MaximumLength(20);
        RuleFor(v => v.Note).MaximumLength(500);
    }
}

public class AddDesignRevisionCommandHandler : IRequestHandler<AddDesignRevisionCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public AddDesignRevisionCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Guid> Handle(AddDesignRevisionCommand request, CancellationToken cancellationToken)
    {
        var design = await _context.Designs.FirstOrDefaultAsync(d => d.Id == request.DesignId, cancellationToken)
            ?? throw new NotFoundException(nameof(Design), request.DesignId);

        var entity = new DesignRevision
        {
            DesignId = request.DesignId,
            Revision = request.Revision,
            Note = request.Note,
            ChangedBy = _currentUserService.UserId,
            Created = DateTimeOffset.UtcNow
        };
        _context.DesignRevisions.Add(entity);

        design.Revision = request.Revision;
        design.LastModified = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }
}

// --- Replace one spec tab's fields ---
public record UpdateDesignSpecsCommand : IRequest
{
    public required Guid DesignId { get; init; }
    public required string Tab { get; init; }
    public required Dictionary<string, string> Fields { get; init; }
}

public class UpdateDesignSpecsCommandValidator : AbstractValidator<UpdateDesignSpecsCommand>
{
    public UpdateDesignSpecsCommandValidator()
    {
        RuleFor(v => v.DesignId).NotEmpty();
        RuleFor(v => v.Tab).NotEmpty().MaximumLength(40);
    }
}

public class UpdateDesignSpecsCommandHandler : IRequestHandler<UpdateDesignSpecsCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateDesignSpecsCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task Handle(UpdateDesignSpecsCommand request, CancellationToken cancellationToken)
    {
        var design = await _context.Designs.FirstOrDefaultAsync(d => d.Id == request.DesignId, cancellationToken)
            ?? throw new NotFoundException(nameof(Design), request.DesignId);

        var specs = new Dictionary<string, Dictionary<string, string>>(design.Specs)
        {
            [request.Tab] = new Dictionary<string, string>(request.Fields)
        };
        design.Specs = specs;
        design.LastModified = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
    }
}

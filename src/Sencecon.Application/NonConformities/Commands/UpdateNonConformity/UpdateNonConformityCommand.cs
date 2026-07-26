using MediatR;
using Microsoft.EntityFrameworkCore;
using Sencecon.Application.Common.Interfaces;
using Sencecon.Domain.Enums;
using Sencecon.Domain.Exceptions;

namespace Sencecon.Application.NonConformities.Commands.UpdateNonConformity;

public record UpdateNonConformityCommand : IRequest
{
    public required Guid Id { get; init; }
    public required string Code { get; init; }
    public required string Description { get; init; }
    public string PlantName { get; init; } = string.Empty;
    public NonConformityStatus Status { get; init; }
}

public class UpdateNonConformityCommandHandler : IRequestHandler<UpdateNonConformityCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateNonConformityCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(UpdateNonConformityCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.NonConformities
            .FirstOrDefaultAsync(n => n.Id == request.Id, cancellationToken);

        if (entity is null)
        {
            throw new NotFoundException(nameof(Domain.Entities.NonConformity), request.Id);
        }

        entity.Code = request.Code;
        entity.Description = request.Description;
        entity.PlantName = request.PlantName;
        entity.Status = request.Status;
        entity.LastModified = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
    }
}

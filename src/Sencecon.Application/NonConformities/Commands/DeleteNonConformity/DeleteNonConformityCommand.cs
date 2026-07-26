using MediatR;
using Microsoft.EntityFrameworkCore;
using Sencecon.Application.Common.Interfaces;
using Sencecon.Domain.Exceptions;

namespace Sencecon.Application.NonConformities.Commands.DeleteNonConformity;

public record DeleteNonConformityCommand : IRequest
{
    public required Guid Id { get; init; }
}

public class DeleteNonConformityCommandHandler : IRequestHandler<DeleteNonConformityCommand>
{
    private readonly IApplicationDbContext _context;

    public DeleteNonConformityCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(DeleteNonConformityCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.NonConformities
            .FirstOrDefaultAsync(n => n.Id == request.Id, cancellationToken);

        if (entity is null)
        {
            throw new NotFoundException(nameof(Domain.Entities.NonConformity), request.Id);
        }

        _context.NonConformities.Remove(entity);

        await _context.SaveChangesAsync(cancellationToken);
    }
}

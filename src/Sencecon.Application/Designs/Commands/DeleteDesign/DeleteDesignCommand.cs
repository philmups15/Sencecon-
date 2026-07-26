using MediatR;
using Microsoft.EntityFrameworkCore;
using Sencecon.Application.Common.Interfaces;
using Sencecon.Domain.Exceptions;

namespace Sencecon.Application.Designs.Commands.DeleteDesign;

public record DeleteDesignCommand : IRequest
{
    public required Guid Id { get; init; }
}

public class DeleteDesignCommandHandler : IRequestHandler<DeleteDesignCommand>
{
    private readonly IApplicationDbContext _context;

    public DeleteDesignCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(DeleteDesignCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Designs
            .FirstOrDefaultAsync(d => d.Id == request.Id, cancellationToken);

        if (entity is null)
        {
            throw new NotFoundException(nameof(Domain.Entities.Design), request.Id);
        }

        _context.Designs.Remove(entity);

        await _context.SaveChangesAsync(cancellationToken);
    }
}

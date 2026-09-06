using MediatR;
using Microsoft.EntityFrameworkCore;
using Sencecon.Application.Common;
using Sencecon.Application.Common.Interfaces;
using Sencecon.Domain.Entities;
using Sencecon.Domain.Enums;
using Sencecon.Domain.Exceptions;

namespace Sencecon.Application.NonConformities.Commands.CreateNonConformity;

public record CreateNonConformityCommand : IRequest<Guid>
{
    public required string Description { get; init; }
    public string PlantName { get; init; } = string.Empty;
    public NonConformityStatus Status { get; init; }
    public Guid? PlantId { get; init; }
}

public class CreateNonConformityCommandHandler : IRequestHandler<CreateNonConformityCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public CreateNonConformityCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreateNonConformityCommand request, CancellationToken cancellationToken)
    {
        if (request.PlantId.HasValue)
        {
            var plantExists = await _context.Plants
                .AnyAsync(p => p.Id == request.PlantId.Value, cancellationToken);

            if (!plantExists)
            {
                throw new NotFoundException(nameof(Domain.Entities.Plant), request.PlantId.Value);
            }
        }

        var code = await EntityCodeGenerator.GenerateNextCodeAsync(_context, "NC-", cancellationToken);

        var entity = new NonConformity
        {
            Code = code,
            Description = request.Description,
            PlantName = request.PlantName,
            Status = request.Status,
            PlantId = request.PlantId,
            Created = DateTimeOffset.UtcNow
        };

        _context.NonConformities.Add(entity);

        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}

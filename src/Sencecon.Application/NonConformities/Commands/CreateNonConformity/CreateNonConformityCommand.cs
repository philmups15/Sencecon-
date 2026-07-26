using MediatR;
using Sencecon.Application.Common.Interfaces;
using Sencecon.Domain.Entities;
using Sencecon.Domain.Enums;

namespace Sencecon.Application.NonConformities.Commands.CreateNonConformity;

public record CreateNonConformityCommand : IRequest<Guid>
{
    public required string Code { get; init; }
    public required string Description { get; init; }
    public string PlantName { get; init; } = string.Empty;
    public NonConformityStatus Status { get; init; }
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
        var entity = new NonConformity
        {
            Code = request.Code,
            Description = request.Description,
            PlantName = request.PlantName,
            Status = request.Status,
            Created = DateTimeOffset.UtcNow
        };

        _context.NonConformities.Add(entity);

        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}

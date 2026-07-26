using MediatR;
using Microsoft.EntityFrameworkCore;
using Sencecon.Application.Common.Interfaces;
using Sencecon.Application.NonConformities.Queries.GetNonConformities;
using Sencecon.Domain.Exceptions;

namespace Sencecon.Application.NonConformities.Queries.GetNonConformityById;

public record GetNonConformityByIdQuery : IRequest<NonConformityDto>
{
    public required Guid Id { get; init; }
}

public class GetNonConformityByIdQueryHandler : IRequestHandler<GetNonConformityByIdQuery, NonConformityDto>
{
    private readonly IApplicationDbContext _context;

    public GetNonConformityByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<NonConformityDto> Handle(GetNonConformityByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _context.NonConformities
            .FirstOrDefaultAsync(n => n.Id == request.Id, cancellationToken);

        if (entity is null)
        {
            throw new NotFoundException(nameof(Domain.Entities.NonConformity), request.Id);
        }

        return new NonConformityDto
        {
            Id = entity.Id,
            Code = entity.Code,
            Description = entity.Description,
            PlantName = entity.PlantName,
            Status = entity.Status,
            Created = entity.Created
        };
    }
}

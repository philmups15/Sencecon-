using MediatR;
using Microsoft.EntityFrameworkCore;
using Sencecon.Application.Common.Interfaces;
using Sencecon.Application.Designs.Queries.GetDesigns;
using Sencecon.Domain.Exceptions;

namespace Sencecon.Application.Designs.Queries.GetDesignById;

public record GetDesignByIdQuery : IRequest<DesignDto>
{
    public required Guid Id { get; init; }
}

public class GetDesignByIdQueryHandler : IRequestHandler<GetDesignByIdQuery, DesignDto>
{
    private readonly IApplicationDbContext _context;

    public GetDesignByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<DesignDto> Handle(GetDesignByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _context.Designs
            .Include(d => d.Project)
            .Include(d => d.Survey)
                .ThenInclude(s => s!.Project)
            .FirstOrDefaultAsync(d => d.Id == request.Id, cancellationToken);

        if (entity is null)
        {
            throw new NotFoundException(nameof(Domain.Entities.Design), request.Id);
        }

        return new DesignDto
        {
            Id = entity.Id,
            Code = entity.Code,
            ProjectName = entity.Project?.Name ?? entity.Survey?.Project?.Name ?? entity.ProjectName,
            Status = entity.Status,
            Revision = entity.Revision,
            SurveyId = entity.SurveyId,
            SurveyCode = entity.Survey?.Code,
            ProjectId = entity.ProjectId ?? entity.Survey?.ProjectId,
            Created = entity.Created
        };
    }
}

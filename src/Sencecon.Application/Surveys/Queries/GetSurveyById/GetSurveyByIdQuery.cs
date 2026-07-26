using MediatR;
using Microsoft.EntityFrameworkCore;
using Sencecon.Application.Common.Interfaces;
using Sencecon.Application.Surveys.Queries.GetSurveys;
using Sencecon.Domain.Exceptions;

namespace Sencecon.Application.Surveys.Queries.GetSurveyById;

public record GetSurveyByIdQuery : IRequest<SurveyDto>
{
    public required Guid Id { get; init; }
}

public class GetSurveyByIdQueryHandler : IRequestHandler<GetSurveyByIdQuery, SurveyDto>
{
    private readonly IApplicationDbContext _context;

    public GetSurveyByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SurveyDto> Handle(GetSurveyByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _context.Surveys
            .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);

        if (entity is null)
        {
            throw new NotFoundException(nameof(Domain.Entities.Survey), request.Id);
        }

        return new SurveyDto
        {
            Id = entity.Id,
            Code = entity.Code,
            PlantName = entity.PlantName,
            Status = entity.Status,
            Progress = entity.Progress,
            Surveyor = entity.Surveyor,
            Date = entity.Date
        };
    }
}

using MediatR;
using Microsoft.EntityFrameworkCore;
using Sencecon.Application.Common.Interfaces;

namespace Sencecon.Application.Surveys.Queries.GetSurveys;

public record GetSurveysQuery : IRequest<IReadOnlyList<SurveyDto>>;

public class GetSurveysQueryHandler : IRequestHandler<GetSurveysQuery, IReadOnlyList<SurveyDto>>
{
    private readonly IApplicationDbContext _context;

    public GetSurveysQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<SurveyDto>> Handle(GetSurveysQuery request, CancellationToken cancellationToken)
    {
        return await _context.Surveys
            .OrderByDescending(s => s.Date)
            .Select(s => new SurveyDto
            {
                Id = s.Id,
                Code = s.Code,
                PlantName = s.PlantName,
                Status = s.Status,
                Progress = s.Progress,
                Surveyor = s.Surveyor,
                Date = s.Date
            })
            .ToListAsync(cancellationToken);
    }
}

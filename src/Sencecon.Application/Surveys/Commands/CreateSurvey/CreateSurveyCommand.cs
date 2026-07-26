using MediatR;
using Sencecon.Application.Common.Interfaces;
using Sencecon.Domain.Entities;
using Sencecon.Domain.Enums;

namespace Sencecon.Application.Surveys.Commands.CreateSurvey;

public record CreateSurveyCommand : IRequest<Guid>
{
    public required string Code { get; init; }
    public required string PlantName { get; init; }
    public SurveyStatus Status { get; init; }
    public int Progress { get; init; }
    public string Surveyor { get; init; } = string.Empty;
    public DateTimeOffset Date { get; init; }
}

public class CreateSurveyCommandHandler : IRequestHandler<CreateSurveyCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public CreateSurveyCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreateSurveyCommand request, CancellationToken cancellationToken)
    {
        var entity = new Survey
        {
            Code = request.Code,
            PlantName = request.PlantName,
            Status = request.Status,
            Progress = request.Progress,
            Surveyor = request.Surveyor,
            Date = request.Date,
            Created = DateTimeOffset.UtcNow
        };

        _context.Surveys.Add(entity);

        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}

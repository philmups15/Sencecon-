using MediatR;
using Microsoft.EntityFrameworkCore;
using Sencecon.Application.Common.Interfaces;
using Sencecon.Domain.Enums;
using Sencecon.Domain.Exceptions;

namespace Sencecon.Application.Surveys.Commands.UpdateSurvey;

public record UpdateSurveyCommand : IRequest
{
    public required Guid Id { get; init; }
    public required string Code { get; init; }
    public required string PlantName { get; init; }
    public SurveyStatus Status { get; init; }
    public int Progress { get; init; }
    public string Surveyor { get; init; } = string.Empty;
    public DateTimeOffset Date { get; init; }
}

public class UpdateSurveyCommandHandler : IRequestHandler<UpdateSurveyCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateSurveyCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(UpdateSurveyCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Surveys
            .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);

        if (entity is null)
        {
            throw new NotFoundException(nameof(Domain.Entities.Survey), request.Id);
        }

        entity.Code = request.Code;
        entity.PlantName = request.PlantName;
        entity.Status = request.Status;
        entity.Progress = request.Progress;
        entity.Surveyor = request.Surveyor;
        entity.Date = request.Date;
        entity.LastModified = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
    }
}

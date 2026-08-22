using MediatR;
using Microsoft.EntityFrameworkCore;
using Sencecon.Application.Common;
using Sencecon.Application.Common.Interfaces;
using Sencecon.Domain.Entities;
using Sencecon.Domain.Enums;
using Sencecon.Domain.Exceptions;

namespace Sencecon.Application.Surveys.Commands.CreateSurvey;

public record CreateSurveyCommand : IRequest<Guid>
{
    public required string PlantName { get; init; }
    public SurveyStatus Status { get; init; }
    public int Progress { get; init; }
    public string Surveyor { get; init; } = string.Empty;
    public DateTimeOffset Date { get; init; }
    public Guid? ProjectId { get; init; }
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
        if (request.ProjectId.HasValue)
        {
            var projectExists = await _context.Projects
                .AnyAsync(p => p.Id == request.ProjectId.Value, cancellationToken);

            if (!projectExists)
            {
                throw new NotFoundException(nameof(Domain.Entities.Project), request.ProjectId.Value);
            }
        }

        var code = await EntityCodeGenerator.GenerateNextCodeAsync(_context.Surveys.Select(s => s.Code), "SUR-", cancellationToken);

        var entity = new Survey
        {
            Code = code,
            PlantName = request.PlantName,
            Status = request.Status,
            Progress = request.Progress,
            Surveyor = request.Surveyor,
            Date = request.Date,
            ProjectId = request.ProjectId,
            Created = DateTimeOffset.UtcNow
        };

        _context.Surveys.Add(entity);

        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}

using MediatR;
using Microsoft.EntityFrameworkCore;
using Sencecon.Application.Common;
using Sencecon.Application.Common.Interfaces;
using Sencecon.Domain.Entities;
using Sencecon.Domain.Enums;
using Sencecon.Domain.Exceptions;

namespace Sencecon.Application.Designs.Commands.CreateDesign;

public record CreateDesignCommand : IRequest<Guid>
{
    public required string ProjectName { get; init; }
    public DesignStatus Status { get; init; }
    public string Revision { get; init; } = string.Empty;
    public Guid? SurveyId { get; init; }
    public Guid? ProjectId { get; init; }
}

public class CreateDesignCommandHandler : IRequestHandler<CreateDesignCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public CreateDesignCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreateDesignCommand request, CancellationToken cancellationToken)
    {
        if (request.SurveyId.HasValue)
        {
            var surveyExists = await _context.Surveys
                .AnyAsync(s => s.Id == request.SurveyId.Value, cancellationToken);

            if (!surveyExists)
            {
                throw new NotFoundException(nameof(Domain.Entities.Survey), request.SurveyId.Value);
            }
        }

        if (request.ProjectId.HasValue)
        {
            var projectExists = await _context.Projects
                .AnyAsync(p => p.Id == request.ProjectId.Value, cancellationToken);

            if (!projectExists)
            {
                throw new NotFoundException(nameof(Domain.Entities.Project), request.ProjectId.Value);
            }
        }

        var code = await EntityCodeGenerator.GenerateNextCodeAsync(_context.Designs.Select(d => d.Code), "DSN-", cancellationToken);

        var entity = new Design
        {
            Code = code,
            ProjectName = request.ProjectName,
            Status = request.Status,
            Revision = request.Revision,
            SurveyId = request.SurveyId,
            ProjectId = request.ProjectId,
            Created = DateTimeOffset.UtcNow
        };

        _context.Designs.Add(entity);

        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}

using MediatR;
using Microsoft.EntityFrameworkCore;
using Sencecon.Application.Common.Interfaces;
using Sencecon.Domain.Entities;
using Sencecon.Domain.Enums;
using Sencecon.Domain.Exceptions;

namespace Sencecon.Application.Designs.Commands.CreateDesign;

public record CreateDesignCommand : IRequest<Guid>
{
    public required string Code { get; init; }
    public required string ProjectName { get; init; }
    public DesignStatus Status { get; init; }
    public string Revision { get; init; } = string.Empty;
    public Guid? SurveyId { get; init; }
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

        var entity = new Design
        {
            Code = request.Code,
            ProjectName = request.ProjectName,
            Status = request.Status,
            Revision = request.Revision,
            SurveyId = request.SurveyId,
            Created = DateTimeOffset.UtcNow
        };

        _context.Designs.Add(entity);

        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}

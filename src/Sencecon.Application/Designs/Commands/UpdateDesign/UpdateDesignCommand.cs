using MediatR;
using Microsoft.EntityFrameworkCore;
using Sencecon.Application.Common.Interfaces;
using Sencecon.Domain.Enums;
using Sencecon.Domain.Exceptions;

namespace Sencecon.Application.Designs.Commands.UpdateDesign;

public record UpdateDesignCommand : IRequest
{
    public required Guid Id { get; init; }
    public required string Code { get; init; }
    public required string ProjectName { get; init; }
    public DesignStatus Status { get; init; }
    public string Revision { get; init; } = string.Empty;
    public Guid? SurveyId { get; init; }
}

public class UpdateDesignCommandHandler : IRequestHandler<UpdateDesignCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateDesignCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(UpdateDesignCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Designs
            .FirstOrDefaultAsync(d => d.Id == request.Id, cancellationToken);

        if (entity is null)
        {
            throw new NotFoundException(nameof(Domain.Entities.Design), request.Id);
        }

        if (request.SurveyId.HasValue)
        {
            var surveyExists = await _context.Surveys
                .AnyAsync(s => s.Id == request.SurveyId.Value, cancellationToken);

            if (!surveyExists)
            {
                throw new NotFoundException(nameof(Domain.Entities.Survey), request.SurveyId.Value);
            }
        }

        entity.Code = request.Code;
        entity.ProjectName = request.ProjectName;
        entity.Status = request.Status;
        entity.Revision = request.Revision;
        entity.SurveyId = request.SurveyId;
        entity.LastModified = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
    }
}

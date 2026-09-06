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
            .Include(d => d.Attachments)
            .Include(d => d.Revisions)
            .FirstOrDefaultAsync(d => d.Id == request.Id, cancellationToken);

        if (entity is null)
        {
            throw new NotFoundException(nameof(Domain.Entities.Design), request.Id);
        }

        var changerIds = entity.Revisions.Where(r => r.ChangedBy.HasValue).Select(r => r.ChangedBy!.Value).Distinct().ToList();
        var changerNames = await _context.Users
            .Where(u => changerIds.Contains(u.Id))
            .ToDictionaryAsync(u => u.Id, u => u.DisplayName, cancellationToken);

        var uploaderIds = entity.Attachments.Select(a => a.UploadedBy).Distinct().ToList();
        var uploaderNames = await _context.Users
            .Where(u => uploaderIds.Contains(u.Id))
            .ToDictionaryAsync(u => u.Id, u => u.DisplayName, cancellationToken);

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
            Created = entity.Created,
            Specs = entity.Specs,
            Attachments = entity.Attachments.OrderByDescending(a => a.Created).Select(a => new Sencecon.Application.Designs.Queries.GetDesignAttachments.DesignAttachmentDto
            {
                Id = a.Id, Title = a.Title, Version = a.Version, FileName = a.FileName, ContentType = a.ContentType,
                SizeBytes = a.SizeBytes, UploadedBy = a.UploadedBy,
                UploadedByName = uploaderNames.GetValueOrDefault(a.UploadedBy, string.Empty), Created = a.Created
            }).ToList(),
            Revisions = entity.Revisions.OrderByDescending(r => r.Created).Select(r => new DesignRevisionDto
            {
                Id = r.Id, Revision = r.Revision, Note = r.Note,
                ChangedByName = r.ChangedBy.HasValue ? changerNames.GetValueOrDefault(r.ChangedBy.Value) : null,
                Created = r.Created
            }).ToList()
        };
    }
}

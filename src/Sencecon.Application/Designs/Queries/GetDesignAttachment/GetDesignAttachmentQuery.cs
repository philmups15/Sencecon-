using MediatR;
using Microsoft.EntityFrameworkCore;
using Sencecon.Application.Common.Interfaces;
using Sencecon.Domain.Entities;
using Sencecon.Domain.Exceptions;

namespace Sencecon.Application.Designs.Queries.GetDesignAttachment;

public record DesignAttachmentContent
{
    public required string FileName { get; init; }
    public required string ContentType { get; init; }
    public required byte[] Content { get; init; }
}

public record GetDesignAttachmentQuery : IRequest<DesignAttachmentContent>
{
    public required Guid DesignId { get; init; }
    public required Guid AttachmentId { get; init; }
}

public class GetDesignAttachmentQueryHandler : IRequestHandler<GetDesignAttachmentQuery, DesignAttachmentContent>
{
    private readonly IApplicationDbContext _context;

    public GetDesignAttachmentQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<DesignAttachmentContent> Handle(GetDesignAttachmentQuery request, CancellationToken cancellationToken)
    {
        var attachment = await _context.DesignAttachments
            .FirstOrDefaultAsync(a => a.Id == request.AttachmentId && a.DesignId == request.DesignId, cancellationToken)
            ?? throw new NotFoundException(nameof(DesignAttachment), request.AttachmentId);

        return new DesignAttachmentContent
        {
            FileName = attachment.FileName,
            ContentType = attachment.ContentType,
            Content = attachment.Content
        };
    }
}

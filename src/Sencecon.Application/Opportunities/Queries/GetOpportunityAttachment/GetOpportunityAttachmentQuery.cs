using MediatR;
using Microsoft.EntityFrameworkCore;
using Sencecon.Application.Common.Interfaces;
using Sencecon.Domain.Entities;
using Sencecon.Domain.Exceptions;

namespace Sencecon.Application.Opportunities.Queries.GetOpportunityAttachment;

public record GetOpportunityAttachmentQuery : IRequest<OpportunityAttachmentContent>
{
    public required Guid OpportunityId { get; init; }
    public required Guid AttachmentId { get; init; }
}

public record OpportunityAttachmentContent
{
    public required string FileName { get; init; }
    public required string ContentType { get; init; }
    public required byte[] Content { get; init; }
}

public class GetOpportunityAttachmentQueryHandler : IRequestHandler<GetOpportunityAttachmentQuery, OpportunityAttachmentContent>
{
    private readonly IApplicationDbContext _context;

    public GetOpportunityAttachmentQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<OpportunityAttachmentContent> Handle(GetOpportunityAttachmentQuery request, CancellationToken cancellationToken)
    {
        var attachment = await _context.OpportunityAttachments
            .FirstOrDefaultAsync(a => a.Id == request.AttachmentId && a.OpportunityId == request.OpportunityId, cancellationToken);

        if (attachment is null)
        {
            throw new NotFoundException(nameof(OpportunityAttachment), request.AttachmentId);
        }

        return new OpportunityAttachmentContent
        {
            FileName = attachment.FileName,
            ContentType = attachment.ContentType,
            Content = attachment.Content
        };
    }
}

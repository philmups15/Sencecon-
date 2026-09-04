using MediatR;
using Microsoft.EntityFrameworkCore;
using Sencecon.Application.Common.Interfaces;
using Sencecon.Domain.Entities;
using Sencecon.Domain.Exceptions;

namespace Sencecon.Application.Plants.Queries.GetPlantAttachment;

public record GetPlantAttachmentQuery : IRequest<PlantAttachmentContent>
{
    public required Guid PlantId { get; init; }
    public required Guid AttachmentId { get; init; }
}

public record PlantAttachmentContent
{
    public required string FileName { get; init; }
    public required string ContentType { get; init; }
    public required byte[] Content { get; init; }
}

public class GetPlantAttachmentQueryHandler : IRequestHandler<GetPlantAttachmentQuery, PlantAttachmentContent>
{
    private readonly IApplicationDbContext _context;

    public GetPlantAttachmentQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PlantAttachmentContent> Handle(GetPlantAttachmentQuery request, CancellationToken cancellationToken)
    {
        var attachment = await _context.PlantAttachments
            .FirstOrDefaultAsync(a => a.Id == request.AttachmentId && a.PlantId == request.PlantId, cancellationToken);

        if (attachment is null)
        {
            throw new NotFoundException(nameof(PlantAttachment), request.AttachmentId);
        }

        return new PlantAttachmentContent
        {
            FileName = attachment.FileName,
            ContentType = attachment.ContentType,
            Content = attachment.Content
        };
    }
}

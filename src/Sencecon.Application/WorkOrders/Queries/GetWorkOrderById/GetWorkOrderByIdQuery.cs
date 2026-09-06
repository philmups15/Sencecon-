using MediatR;
using Microsoft.EntityFrameworkCore;
using Sencecon.Application.Common.Interfaces;
using Sencecon.Application.WorkOrders.Queries.GetWorkOrders;
using Sencecon.Domain.Exceptions;

namespace Sencecon.Application.WorkOrders.Queries.GetWorkOrderById;

public record GetWorkOrderByIdQuery : IRequest<WorkOrderDto>
{
    public required Guid Id { get; init; }
}

public class GetWorkOrderByIdQueryHandler : IRequestHandler<GetWorkOrderByIdQuery, WorkOrderDto>
{
    private readonly IApplicationDbContext _context;

    public GetWorkOrderByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<WorkOrderDto> Handle(GetWorkOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _context.WorkOrders
            .Include(w => w.Plant)
            .Include(w => w.ChecklistItems)
            .Include(w => w.Parts)
            .Include(w => w.Labour)
            .Include(w => w.Attachments)
            .FirstOrDefaultAsync(w => w.Id == request.Id, cancellationToken);

        if (entity is null)
        {
            throw new NotFoundException(nameof(Domain.Entities.WorkOrder), request.Id);
        }

        var uploaderIds = entity.Attachments.Select(a => a.UploadedBy).Distinct().ToList();
        var uploaderNames = await _context.Users
            .Where(u => uploaderIds.Contains(u.Id))
            .ToDictionaryAsync(u => u.Id, u => u.DisplayName, cancellationToken);

        return new WorkOrderDto
        {
            Id = entity.Id,
            Code = entity.Code,
            Title = entity.Title,
            Type = entity.Type,
            Priority = entity.Priority,
            Assignee = entity.Assignee,
            Status = entity.Status,
            DueDate = entity.DueDate,
            PlantId = entity.PlantId,
            PlantName = entity.Plant!.Name,
            Created = entity.Created,
            ChecklistItems = entity.ChecklistItems.OrderBy(c => c.Order).ThenBy(c => c.Text)
                .Select(c => new WorkOrderChecklistItemDto { Id = c.Id, Text = c.Text, IsDone = c.IsDone, Order = c.Order }).ToList(),
            Parts = entity.Parts.OrderBy(p => p.PartName)
                .Select(p => new WorkOrderPartDto { Id = p.Id, PartName = p.PartName, Quantity = p.Quantity, Notes = p.Notes }).ToList(),
            Labour = entity.Labour.OrderByDescending(l => l.WorkDate ?? DateTimeOffset.MinValue)
                .Select(l => new WorkOrderLabourDto { Id = l.Id, PersonName = l.PersonName, Hours = l.Hours, WorkDate = l.WorkDate, Notes = l.Notes }).ToList(),
            Attachments = entity.Attachments.OrderByDescending(a => a.Created)
                .Select(a => new Sencecon.Application.WorkOrders.Commands.UploadWorkOrderAttachments.WorkOrderAttachmentDto
                {
                    Id = a.Id, Title = a.Title, Version = a.Version, FileName = a.FileName, ContentType = a.ContentType,
                    SizeBytes = a.SizeBytes, UploadedByName = uploaderNames.GetValueOrDefault(a.UploadedBy, string.Empty), Created = a.Created
                }).ToList()
        };
    }
}

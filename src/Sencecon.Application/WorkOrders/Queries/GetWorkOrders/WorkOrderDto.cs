using Sencecon.Domain.Enums;
using Sencecon.Application.WorkOrders.Commands.UploadWorkOrderAttachments;

namespace Sencecon.Application.WorkOrders.Queries.GetWorkOrders;

public record WorkOrderChecklistItemDto
{
    public Guid Id { get; init; }
    public string Text { get; init; } = string.Empty;
    public bool IsDone { get; init; }
    public int Order { get; init; }
}

public record WorkOrderPartDto
{
    public Guid Id { get; init; }
    public string PartName { get; init; } = string.Empty;
    public int Quantity { get; init; }
    public string? Notes { get; init; }
}

public record WorkOrderLabourDto
{
    public Guid Id { get; init; }
    public string PersonName { get; init; } = string.Empty;
    public decimal Hours { get; init; }
    public DateTimeOffset? WorkDate { get; init; }
    public string? Notes { get; init; }
}

public record WorkOrderDto
{
    public Guid Id { get; init; }
    public string Code { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public WorkOrderType Type { get; init; }
    public Priority Priority { get; init; }
    public string Assignee { get; init; } = string.Empty;
    public WorkOrderStatus Status { get; init; }
    public DateTimeOffset? DueDate { get; init; }
    public Guid PlantId { get; init; }
    public string PlantName { get; init; } = string.Empty;
    public DateTimeOffset Created { get; init; }

    // GetWorkOrderByIdQuery only.
    public IReadOnlyList<WorkOrderChecklistItemDto>? ChecklistItems { get; init; }
    public IReadOnlyList<WorkOrderPartDto>? Parts { get; init; }
    public IReadOnlyList<WorkOrderLabourDto>? Labour { get; init; }
    public IReadOnlyList<WorkOrderAttachmentDto>? Attachments { get; init; }
}

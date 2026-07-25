using Sencecon.Domain.Common;

namespace Sencecon.Domain.Entities;

public class TodoItem : BaseAuditableEntity
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsDone { get; set; }
    public DateTimeOffset? DueDate { get; set; }

    public Guid OwnerId { get; set; }
    public User? Owner { get; set; }
}

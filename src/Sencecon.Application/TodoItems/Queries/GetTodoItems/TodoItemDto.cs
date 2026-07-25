namespace Sencecon.Application.TodoItems.Queries.GetTodoItems;

public record TodoItemDto
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public bool IsDone { get; init; }
    public DateTimeOffset? DueDate { get; init; }
    public DateTimeOffset Created { get; init; }
}

using MediatR;
using Microsoft.EntityFrameworkCore;
using Sencecon.Application.Common.Interfaces;

namespace Sencecon.Application.TodoItems.Queries.GetTodoItems;

public record GetTodoItemsQuery : IRequest<IReadOnlyList<TodoItemDto>>
{
    public required Guid OwnerId { get; init; }
}

public class GetTodoItemsQueryHandler : IRequestHandler<GetTodoItemsQuery, IReadOnlyList<TodoItemDto>>
{
    private readonly IApplicationDbContext _context;

    public GetTodoItemsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<TodoItemDto>> Handle(GetTodoItemsQuery request, CancellationToken cancellationToken)
    {
        return await _context.TodoItems
            .Where(t => t.OwnerId == request.OwnerId)
            .OrderByDescending(t => t.Created)
            .Select(t => new TodoItemDto
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                IsDone = t.IsDone,
                DueDate = t.DueDate,
                Created = t.Created
            })
            .ToListAsync(cancellationToken);
    }
}

using MediatR;
using Microsoft.EntityFrameworkCore;
using Sencecon.Application.Common.Interfaces;
using Sencecon.Application.TodoItems.Queries.GetTodoItems;
using Sencecon.Domain.Exceptions;

namespace Sencecon.Application.TodoItems.Queries.GetTodoItemById;

public record GetTodoItemByIdQuery : IRequest<TodoItemDto>
{
    public required Guid Id { get; init; }
    public required Guid RequestingUserId { get; init; }
}

public class GetTodoItemByIdQueryHandler : IRequestHandler<GetTodoItemByIdQuery, TodoItemDto>
{
    private readonly IApplicationDbContext _context;

    public GetTodoItemByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<TodoItemDto> Handle(GetTodoItemByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _context.TodoItems
            .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

        if (entity is null)
        {
            throw new NotFoundException(nameof(Domain.Entities.TodoItem), request.Id);
        }

        if (entity.OwnerId != request.RequestingUserId)
        {
            throw new ForbiddenAccessException();
        }

        return new TodoItemDto
        {
            Id = entity.Id,
            Title = entity.Title,
            Description = entity.Description,
            IsDone = entity.IsDone,
            DueDate = entity.DueDate,
            Created = entity.Created
        };
    }
}

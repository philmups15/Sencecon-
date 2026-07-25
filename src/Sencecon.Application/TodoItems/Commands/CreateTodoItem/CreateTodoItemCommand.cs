using MediatR;
using Sencecon.Application.Common.Interfaces;
using Sencecon.Domain.Entities;

namespace Sencecon.Application.TodoItems.Commands.CreateTodoItem;

public record CreateTodoItemCommand : IRequest<Guid>
{
    public required string Title { get; init; }
    public string? Description { get; init; }
    public DateTimeOffset? DueDate { get; init; }
    public required Guid OwnerId { get; init; }
}

public class CreateTodoItemCommandHandler : IRequestHandler<CreateTodoItemCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public CreateTodoItemCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreateTodoItemCommand request, CancellationToken cancellationToken)
    {
        var entity = new TodoItem
        {
            Title = request.Title,
            Description = request.Description,
            DueDate = request.DueDate,
            OwnerId = request.OwnerId,
            Created = DateTimeOffset.UtcNow
        };

        _context.TodoItems.Add(entity);

        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}

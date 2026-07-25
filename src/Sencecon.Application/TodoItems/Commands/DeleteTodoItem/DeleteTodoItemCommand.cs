using MediatR;
using Microsoft.EntityFrameworkCore;
using Sencecon.Application.Common.Interfaces;
using Sencecon.Domain.Exceptions;

namespace Sencecon.Application.TodoItems.Commands.DeleteTodoItem;

public record DeleteTodoItemCommand : IRequest
{
    public required Guid Id { get; init; }
    public required Guid RequestingUserId { get; init; }
}

public class DeleteTodoItemCommandHandler : IRequestHandler<DeleteTodoItemCommand>
{
    private readonly IApplicationDbContext _context;

    public DeleteTodoItemCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(DeleteTodoItemCommand request, CancellationToken cancellationToken)
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

        _context.TodoItems.Remove(entity);

        await _context.SaveChangesAsync(cancellationToken);
    }
}

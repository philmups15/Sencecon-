using MediatR;
using Microsoft.EntityFrameworkCore;
using Sencecon.Application.Common.Interfaces;
using Sencecon.Domain.Exceptions;

namespace Sencecon.Application.TodoItems.Commands.UpdateTodoItem;

public record UpdateTodoItemCommand : IRequest
{
    public required Guid Id { get; init; }
    public required string Title { get; init; }
    public string? Description { get; init; }
    public bool IsDone { get; init; }
    public DateTimeOffset? DueDate { get; init; }
    public required Guid RequestingUserId { get; init; }
}

public class UpdateTodoItemCommandHandler : IRequestHandler<UpdateTodoItemCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateTodoItemCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(UpdateTodoItemCommand request, CancellationToken cancellationToken)
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

        entity.Title = request.Title;
        entity.Description = request.Description;
        entity.IsDone = request.IsDone;
        entity.DueDate = request.DueDate;
        entity.LastModified = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
    }
}

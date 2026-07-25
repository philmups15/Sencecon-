using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sencecon.Application.Common.Interfaces;
using Sencecon.Application.TodoItems.Commands.CreateTodoItem;
using Sencecon.Application.TodoItems.Commands.DeleteTodoItem;
using Sencecon.Application.TodoItems.Commands.UpdateTodoItem;
using Sencecon.Application.TodoItems.Queries.GetTodoItemById;
using Sencecon.Application.TodoItems.Queries.GetTodoItems;

namespace Sencecon.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TodoItemsController : ControllerBase
{
    private readonly ISender _sender;
    private readonly ICurrentUserService _currentUserService;

    public TodoItemsController(ISender sender, ICurrentUserService currentUserService)
    {
        _sender = sender;
        _currentUserService = currentUserService;
    }

    private Guid CurrentUserId => _currentUserService.UserId
        ?? throw new UnauthorizedAccessException("No authenticated user.");

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<TodoItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<TodoItemDto>>> GetAll()
    {
        var result = await _sender.Send(new GetTodoItemsQuery { OwnerId = CurrentUserId });
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(TodoItemDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<TodoItemDto>> GetById(Guid id)
    {
        var result = await _sender.Send(new GetTodoItemByIdQuery { Id = id, RequestingUserId = CurrentUserId });
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    public async Task<ActionResult<Guid>> Create(CreateTodoItemRequest request)
    {
        var id = await _sender.Send(new CreateTodoItemCommand
        {
            Title = request.Title,
            Description = request.Description,
            DueDate = request.DueDate,
            OwnerId = CurrentUserId
        });

        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Update(Guid id, UpdateTodoItemRequest request)
    {
        await _sender.Send(new UpdateTodoItemCommand
        {
            Id = id,
            Title = request.Title,
            Description = request.Description,
            IsDone = request.IsDone,
            DueDate = request.DueDate,
            RequestingUserId = CurrentUserId
        });

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _sender.Send(new DeleteTodoItemCommand { Id = id, RequestingUserId = CurrentUserId });
        return NoContent();
    }
}

public record CreateTodoItemRequest(string Title, string? Description, DateTimeOffset? DueDate);

public record UpdateTodoItemRequest(string Title, string? Description, bool IsDone, DateTimeOffset? DueDate);

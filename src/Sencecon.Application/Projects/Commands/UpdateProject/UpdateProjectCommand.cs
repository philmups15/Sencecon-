using MediatR;
using Microsoft.EntityFrameworkCore;
using Sencecon.Application.Common.Interfaces;
using Sencecon.Domain.Enums;
using Sencecon.Domain.Exceptions;

namespace Sencecon.Application.Projects.Commands.UpdateProject;

public record UpdateProjectCommand : IRequest
{
    public required Guid Id { get; init; }
    public required string Code { get; init; }
    public required string Name { get; init; }
    public string Customer { get; init; } = string.Empty;
    public LifecycleStage Stage { get; init; }
    public string ProjectManager { get; init; } = string.Empty;
    public decimal Budget { get; init; }
    public decimal Actual { get; init; }
}

public class UpdateProjectCommandHandler : IRequestHandler<UpdateProjectCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateProjectCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(UpdateProjectCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Projects
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        if (entity is null)
        {
            throw new NotFoundException(nameof(Domain.Entities.Project), request.Id);
        }

        entity.Code = request.Code;
        entity.Name = request.Name;
        entity.Customer = request.Customer;
        entity.Stage = request.Stage;
        entity.ProjectManager = request.ProjectManager;
        entity.Budget = request.Budget;
        entity.Actual = request.Actual;
        entity.LastModified = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
    }
}

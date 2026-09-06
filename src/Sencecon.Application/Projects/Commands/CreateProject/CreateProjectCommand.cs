using MediatR;
using Sencecon.Application.Common;
using Sencecon.Application.Common.Interfaces;
using Sencecon.Domain.Entities;
using Sencecon.Domain.Enums;

namespace Sencecon.Application.Projects.Commands.CreateProject;

public record CreateProjectCommand : IRequest<Guid>
{
    public required string Name { get; init; }
    public string Customer { get; init; } = string.Empty;
    public LifecycleStage Stage { get; init; }
    public string ProjectManager { get; init; } = string.Empty;
    public decimal Budget { get; init; }
    public decimal Actual { get; init; }
}

public class CreateProjectCommandHandler : IRequestHandler<CreateProjectCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public CreateProjectCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
    {
        var code = await EntityCodeGenerator.GenerateNextCodeAsync(_context, "PRJ-", cancellationToken);

        var entity = new Project
        {
            Code = code,
            Name = request.Name,
            Customer = request.Customer,
            Stage = request.Stage,
            ProjectManager = request.ProjectManager,
            Budget = request.Budget,
            Actual = request.Actual,
            Created = DateTimeOffset.UtcNow
        };

        _context.Projects.Add(entity);

        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}

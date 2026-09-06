using MediatR;
using Microsoft.EntityFrameworkCore;
using Sencecon.Application.Common.Interfaces;
using Sencecon.Domain.Enums;
using Sencecon.Domain.Exceptions;

namespace Sencecon.Application.Plants.Queries.GetCommissioningChecklist;

public record CommissioningChecklistRowDto
{
    public Guid? ResultId { get; init; }
    public CommissioningTestCategory Category { get; init; }
    public string TestName { get; init; } = string.Empty;
    public CommissioningResultStatus Result { get; init; }
    public string? Notes { get; init; }
    public DateTimeOffset? RecordedDate { get; init; }
}

public record GetCommissioningChecklistQuery : IRequest<IReadOnlyList<CommissioningChecklistRowDto>>
{
    public required Guid PlantId { get; init; }
}

public class GetCommissioningChecklistQueryHandler : IRequestHandler<GetCommissioningChecklistQuery, IReadOnlyList<CommissioningChecklistRowDto>>
{
    private readonly IApplicationDbContext _context;
    public GetCommissioningChecklistQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<IReadOnlyList<CommissioningChecklistRowDto>> Handle(GetCommissioningChecklistQuery request, CancellationToken cancellationToken)
    {
        var plant = await _context.Plants
            .Where(p => p.Id == request.PlantId)
            .Select(p => new { p.Type })
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Plant), request.PlantId);

        var plantType = (int)plant.Type;

        var templates = await _context.CommissioningTestTemplates
            .Where(t => t.IsActive)
            .OrderBy(t => t.Order).ThenBy(t => t.TestName)
            .ToListAsync(cancellationToken);

        var applicable = templates
            .Where(t => t.AppliesToTypes.Length == 0 || t.AppliesToTypes.Contains(plantType))
            .ToList();

        var results = await _context.CommissioningTestResults
            .Where(r => r.PlantId == request.PlantId)
            .ToListAsync(cancellationToken);

        return applicable.Select(t =>
        {
            var match = results.FirstOrDefault(r => r.Category == t.Category && r.TestName == t.TestName);
            return new CommissioningChecklistRowDto
            {
                ResultId = match?.Id,
                Category = t.Category,
                TestName = t.TestName,
                Result = match?.Result ?? CommissioningResultStatus.Pending,
                Notes = match?.Notes,
                RecordedDate = match is null ? null : (match.LastModified ?? match.Created)
            };
        }).ToList();
    }
}

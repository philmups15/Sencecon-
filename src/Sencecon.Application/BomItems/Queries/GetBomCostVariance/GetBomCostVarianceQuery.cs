using MediatR;
using Microsoft.EntityFrameworkCore;
using Sencecon.Application.Common.Interfaces;
using Sencecon.Domain.Enums;

namespace Sencecon.Application.BomItems.Queries.GetBomCostVariance;

public record BomCostVarianceRowDto
{
    public BomCategory Category { get; init; }
    public decimal Budget { get; init; }
    public decimal Actual { get; init; }
}

public record GetBomCostVarianceQuery : IRequest<IReadOnlyList<BomCostVarianceRowDto>>
{
    public required Guid ProjectId { get; init; }
}

public class GetBomCostVarianceQueryHandler : IRequestHandler<GetBomCostVarianceQuery, IReadOnlyList<BomCostVarianceRowDto>>
{
    private readonly IApplicationDbContext _context;

    public GetBomCostVarianceQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<BomCostVarianceRowDto>> Handle(GetBomCostVarianceQuery request, CancellationToken cancellationToken)
    {
        var actuals = await _context.BomItems
            .Where(b => b.ProjectId == request.ProjectId)
            .GroupBy(b => b.Category)
            .Select(g => new { Category = g.Key, Actual = g.Sum(b => b.UnitCost * b.Quantity) })
            .ToDictionaryAsync(x => x.Category, x => x.Actual, cancellationToken);

        var budgets = await _context.ProjectBudgetLines
            .Where(l => l.ProjectId == request.ProjectId && l.Category != null)
            .GroupBy(l => l.Category!.Value)
            .Select(g => new { Category = g.Key, Budget = g.Sum(l => l.BudgetAmount) })
            .ToDictionaryAsync(x => x.Category, x => x.Budget, cancellationToken);

        return Enum.GetValues<BomCategory>()
            .Where(c => actuals.ContainsKey(c) || budgets.ContainsKey(c))
            .Select(c => new BomCostVarianceRowDto
            {
                Category = c,
                Actual = actuals.GetValueOrDefault(c),
                Budget = budgets.GetValueOrDefault(c)
            })
            .ToList();
    }
}

using MediatR;
using Microsoft.EntityFrameworkCore;
using Sencecon.Application.Common.Interfaces;
using Sencecon.Domain.Enums;

namespace Sencecon.Application.Plants.Queries.GetCommissioningTestResults;

public record CommissioningTestResultDto
{
    public Guid Id { get; init; }
    public CommissioningTestCategory Category { get; init; }
    public string TestName { get; init; } = string.Empty;
    public CommissioningResultStatus Result { get; init; }
    public string? Notes { get; init; }
    public DateTimeOffset RecordedDate { get; init; }
}

public record GetCommissioningTestResultsQuery : IRequest<IReadOnlyList<CommissioningTestResultDto>>
{
    public required Guid PlantId { get; init; }
}

public class GetCommissioningTestResultsQueryHandler : IRequestHandler<GetCommissioningTestResultsQuery, IReadOnlyList<CommissioningTestResultDto>>
{
    private readonly IApplicationDbContext _context;

    public GetCommissioningTestResultsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<CommissioningTestResultDto>> Handle(GetCommissioningTestResultsQuery request, CancellationToken cancellationToken)
    {
        return await _context.CommissioningTestResults
            .Where(t => t.PlantId == request.PlantId)
            .Select(t => new CommissioningTestResultDto
            {
                Id = t.Id,
                Category = t.Category,
                TestName = t.TestName,
                Result = t.Result,
                Notes = t.Notes,
                RecordedDate = t.LastModified ?? t.Created
            })
            .ToListAsync(cancellationToken);
    }
}

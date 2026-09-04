using MediatR;
using Microsoft.EntityFrameworkCore;
using Sencecon.Application.Common.Interfaces;
using Sencecon.Application.Plants.Queries.GetCommissioningTestResults;
using Sencecon.Domain.Entities;
using Sencecon.Domain.Enums;
using Sencecon.Domain.Exceptions;

namespace Sencecon.Application.Plants.Commands.RecordCommissioningTestResult;

public record RecordCommissioningTestResultCommand : IRequest<CommissioningTestResultDto>
{
    public required Guid PlantId { get; init; }
    public required CommissioningTestCategory Category { get; init; }
    public required string TestName { get; init; }
    public required CommissioningResultStatus Result { get; init; }
    public string? Notes { get; init; }
}

public class RecordCommissioningTestResultCommandHandler : IRequestHandler<RecordCommissioningTestResultCommand, CommissioningTestResultDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public RecordCommissioningTestResultCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<CommissioningTestResultDto> Handle(RecordCommissioningTestResultCommand request, CancellationToken cancellationToken)
    {
        var plantExists = await _context.Plants
            .AnyAsync(p => p.Id == request.PlantId, cancellationToken);

        if (!plantExists)
        {
            throw new NotFoundException(nameof(Plant), request.PlantId);
        }

        var entity = await _context.CommissioningTestResults
            .FirstOrDefaultAsync(
                t => t.PlantId == request.PlantId && t.Category == request.Category && t.TestName == request.TestName,
                cancellationToken);

        var currentUserId = _currentUserService.UserId;
        var now = DateTimeOffset.UtcNow;

        if (entity is null)
        {
            entity = new CommissioningTestResult
            {
                PlantId = request.PlantId,
                Category = request.Category,
                TestName = request.TestName,
                Result = request.Result,
                Notes = request.Notes,
                RecordedBy = currentUserId,
                Created = now
            };
            _context.CommissioningTestResults.Add(entity);
        }
        else
        {
            entity.Result = request.Result;
            entity.Notes = request.Notes;
            entity.RecordedBy = currentUserId;
            entity.LastModified = now;
        }

        await _context.SaveChangesAsync(cancellationToken);

        return new CommissioningTestResultDto
        {
            Id = entity.Id,
            Category = entity.Category,
            TestName = entity.TestName,
            Result = entity.Result,
            Notes = entity.Notes,
            RecordedDate = entity.LastModified ?? entity.Created
        };
    }
}

using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Sencecon.Application.Common;
using Sencecon.Application.Common.Interfaces;
using Sencecon.Domain.Enums;
using Sencecon.Domain.Exceptions;
using HandoverEntity = Sencecon.Domain.Entities.Handover;

namespace Sencecon.Application.Plants.Handover;

public record HandoverDto
{
    public Guid PlantId { get; init; }
    public HandoverStatus Status { get; init; }
    public string? SignedOffByName { get; init; }
    public DateTimeOffset? SignedOffDate { get; init; }
    public DateTimeOffset? AcceptanceDate { get; init; }
    public string? Notes { get; init; }
    public bool HasCertificate { get; init; }
    public bool CommissioningComplete { get; init; }
    public IReadOnlyList<string> OutstandingTests { get; init; } = Array.Empty<string>();
}

// ---- Get ----
public record GetHandoverQuery : IRequest<HandoverDto>
{
    public required Guid PlantId { get; init; }
}

public class GetHandoverQueryHandler : IRequestHandler<GetHandoverQuery, HandoverDto>
{
    private readonly IApplicationDbContext _context;
    public GetHandoverQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<HandoverDto> Handle(GetHandoverQuery request, CancellationToken cancellationToken)
    {
        var plant = await _context.Plants.FirstOrDefaultAsync(p => p.Id == request.PlantId, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Plant), request.PlantId);

        var handover = await _context.Handovers.FirstOrDefaultAsync(h => h.PlantId == request.PlantId, cancellationToken);

        var (complete, missing) = await CommissioningCompletion.EvaluateAsync(_context, plant.Id, plant.Type, cancellationToken);

        string? signedOffByName = null;
        if (handover?.SignedOffBy is { } uid)
        {
            signedOffByName = await _context.Users.Where(u => u.Id == uid).Select(u => u.DisplayName).FirstOrDefaultAsync(cancellationToken);
        }

        return new HandoverDto
        {
            PlantId = request.PlantId,
            Status = handover?.Status ?? HandoverStatus.Draft,
            SignedOffByName = signedOffByName,
            SignedOffDate = handover?.SignedOffDate,
            AcceptanceDate = handover?.AcceptanceDate,
            Notes = handover?.Notes,
            HasCertificate = handover?.CertificateContent is { Length: > 0 },
            CommissioningComplete = complete,
            OutstandingTests = missing
        };
    }
}

// ---- Upsert draft ----
public record UpsertHandoverCommand : IRequest
{
    public required Guid PlantId { get; init; }
    public DateTimeOffset? AcceptanceDate { get; init; }
    public string? Notes { get; init; }
}

public class UpsertHandoverCommandValidator : AbstractValidator<UpsertHandoverCommand>
{
    public UpsertHandoverCommandValidator()
    {
        RuleFor(v => v.PlantId).NotEmpty();
        RuleFor(v => v.Notes).MaximumLength(2000);
    }
}

public class UpsertHandoverCommandHandler : IRequestHandler<UpsertHandoverCommand>
{
    private readonly IApplicationDbContext _context;
    public UpsertHandoverCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task Handle(UpsertHandoverCommand request, CancellationToken cancellationToken)
    {
        if (!await _context.Plants.AnyAsync(p => p.Id == request.PlantId, cancellationToken))
            throw new NotFoundException(nameof(Domain.Entities.Plant), request.PlantId);

        var handover = await _context.Handovers.FirstOrDefaultAsync(h => h.PlantId == request.PlantId, cancellationToken);
        if (handover is null)
        {
            handover = new HandoverEntity { PlantId = request.PlantId, Status = HandoverStatus.Draft, Created = DateTimeOffset.UtcNow };
            _context.Handovers.Add(handover);
        }

        handover.AcceptanceDate = request.AcceptanceDate;
        handover.Notes = request.Notes;
        handover.LastModified = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
    }
}

// ---- Sign off ----
public record SignOffHandoverCommand : IRequest
{
    public required Guid PlantId { get; init; }
}

public class SignOffHandoverCommandHandler : IRequestHandler<SignOffHandoverCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public SignOffHandoverCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task Handle(SignOffHandoverCommand request, CancellationToken cancellationToken)
    {
        var plant = await _context.Plants.FirstOrDefaultAsync(p => p.Id == request.PlantId, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Plant), request.PlantId);

        var (complete, missing) = await CommissioningCompletion.EvaluateAsync(_context, plant.Id, plant.Type, cancellationToken);
        if (!complete)
            throw new ConflictException($"Cannot sign off handover — outstanding commissioning tests: {string.Join(", ", missing)}.");

        var handover = await _context.Handovers.FirstOrDefaultAsync(h => h.PlantId == request.PlantId, cancellationToken);
        if (handover is null)
        {
            handover = new HandoverEntity { PlantId = request.PlantId, Created = DateTimeOffset.UtcNow };
            _context.Handovers.Add(handover);
        }

        handover.Status = HandoverStatus.SignedOff;
        handover.SignedOffBy = _currentUserService.UserId;
        handover.SignedOffDate = DateTimeOffset.UtcNow;
        handover.LastModified = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
    }
}

// ---- Certificate ----
public record UploadHandoverCertificateCommand : IRequest
{
    public required Guid PlantId { get; init; }
    public required string FileName { get; init; }
    public required string ContentType { get; init; }
    public required byte[] Content { get; init; }
}

public class UploadHandoverCertificateCommandValidator : AbstractValidator<UploadHandoverCertificateCommand>
{
    public UploadHandoverCertificateCommandValidator()
    {
        RuleFor(v => v.PlantId).NotEmpty();
        RuleFor(v => v.FileName).NotEmpty().MaximumLength(260);
        RuleFor(v => v.Content.LongLength).LessThanOrEqualTo(10 * 1024 * 1024).WithMessage("Certificate must be 10 MB or smaller.");
    }
}

public class UploadHandoverCertificateCommandHandler : IRequestHandler<UploadHandoverCertificateCommand>
{
    private readonly IApplicationDbContext _context;
    public UploadHandoverCertificateCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task Handle(UploadHandoverCertificateCommand request, CancellationToken cancellationToken)
    {
        if (!await _context.Plants.AnyAsync(p => p.Id == request.PlantId, cancellationToken))
            throw new NotFoundException(nameof(Domain.Entities.Plant), request.PlantId);

        var handover = await _context.Handovers.FirstOrDefaultAsync(h => h.PlantId == request.PlantId, cancellationToken);
        if (handover is null)
        {
            handover = new HandoverEntity { PlantId = request.PlantId, Created = DateTimeOffset.UtcNow };
            _context.Handovers.Add(handover);
        }

        handover.CertificateFileName = request.FileName;
        handover.CertificateContentType = request.ContentType;
        handover.CertificateContent = request.Content;
        handover.LastModified = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
    }
}

public record HandoverCertificateContent
{
    public required string FileName { get; init; }
    public required string ContentType { get; init; }
    public required byte[] Content { get; init; }
}

public record GetHandoverCertificateQuery : IRequest<HandoverCertificateContent>
{
    public required Guid PlantId { get; init; }
}

public class GetHandoverCertificateQueryHandler : IRequestHandler<GetHandoverCertificateQuery, HandoverCertificateContent>
{
    private readonly IApplicationDbContext _context;
    public GetHandoverCertificateQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<HandoverCertificateContent> Handle(GetHandoverCertificateQuery request, CancellationToken cancellationToken)
    {
        var handover = await _context.Handovers.FirstOrDefaultAsync(h => h.PlantId == request.PlantId, cancellationToken);
        if (handover?.CertificateContent is not { Length: > 0 })
            throw new NotFoundException("HandoverCertificate", request.PlantId);

        return new HandoverCertificateContent
        {
            FileName = handover.CertificateFileName ?? "handover-certificate",
            ContentType = handover.CertificateContentType ?? "application/octet-stream",
            Content = handover.CertificateContent
        };
    }
}

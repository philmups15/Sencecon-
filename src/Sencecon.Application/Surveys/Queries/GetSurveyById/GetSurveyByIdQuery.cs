using MediatR;
using Microsoft.EntityFrameworkCore;
using Sencecon.Application.Common.Interfaces;
using Sencecon.Application.Surveys.Queries.GetSurveys;
using Sencecon.Domain.Exceptions;

namespace Sencecon.Application.Surveys.Queries.GetSurveyById;

public record GetSurveyByIdQuery : IRequest<SurveyDto>
{
    public required Guid Id { get; init; }
}

public class GetSurveyByIdQueryHandler : IRequestHandler<GetSurveyByIdQuery, SurveyDto>
{
    private readonly IApplicationDbContext _context;

    public GetSurveyByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SurveyDto> Handle(GetSurveyByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _context.Surveys
            .Include(s => s.Project)
            .Include(s => s.Plant)
            .Include(s => s.Measurements)
            .Include(s => s.Obstructions)
            .Include(s => s.Photos)
            .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);

        if (entity is null)
        {
            throw new NotFoundException(nameof(Domain.Entities.Survey), request.Id);
        }

        var uploaderIds = entity.Photos.Select(p => p.UploadedBy).Distinct().ToList();
        var uploaderNames = await _context.Users
            .Where(u => uploaderIds.Contains(u.Id))
            .ToDictionaryAsync(u => u.Id, u => u.DisplayName, cancellationToken);

        return new SurveyDto
        {
            Id = entity.Id,
            Code = entity.Code,
            PlantName = entity.Plant?.Name ?? entity.PlantName,
            Status = entity.Status,
            Progress = entity.Progress,
            Surveyor = entity.Surveyor,
            Date = entity.Date,
            ProjectId = entity.ProjectId,
            ProjectName = entity.Project?.Name,
            PlantId = entity.PlantId,
            Measurements = entity.Measurements.OrderBy(m => m.Created)
                .Select(m => new SurveyMeasurementDto { Id = m.Id, Field = m.Field, Value = m.Value }).ToList(),
            Obstructions = entity.Obstructions.OrderBy(o => o.Created)
                .Select(o => new SurveyObstructionDto { Id = o.Id, Item = o.Item, Impact = o.Impact }).ToList(),
            Photos = entity.Photos.OrderByDescending(p => p.Created)
                .Select(p => new Sencecon.Application.Surveys.Commands.UploadSurveyPhotos.SurveyPhotoDto
                {
                    Id = p.Id, Title = p.Title, Version = p.Version, FileName = p.FileName, ContentType = p.ContentType,
                    SizeBytes = p.SizeBytes, Gps = p.Gps,
                    UploadedByName = uploaderNames.GetValueOrDefault(p.UploadedBy, string.Empty), Created = p.Created
                }).ToList()
        };
    }
}

using FluentValidation;

namespace Sencecon.Application.Plants.Commands.CreatePlant;

public class CreatePlantCommandValidator : AbstractValidator<CreatePlantCommand>
{
    public CreatePlantCommandValidator()
    {
        RuleFor(v => v.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(v => v.Capacity)
            .MaximumLength(50);

        RuleFor(v => v.Equipment)
            .MaximumLength(200);

        RuleFor(v => v.PerformanceRatio)
            .InclusiveBetween(0, 2)
            .When(v => v.PerformanceRatio.HasValue);

        RuleFor(v => v.Latitude)
            .InclusiveBetween(-90, 90)
            .When(v => v.Latitude.HasValue);

        RuleFor(v => v.Longitude)
            .InclusiveBetween(-180, 180)
            .When(v => v.Longitude.HasValue);
    }
}

using FluentValidation;

namespace Sencecon.Application.Plants.Commands.CreatePlant;

public class CreatePlantCommandValidator : AbstractValidator<CreatePlantCommand>
{
    public CreatePlantCommandValidator()
    {
        RuleFor(v => v.Code)
            .NotEmpty()
            .MaximumLength(20);

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
    }
}

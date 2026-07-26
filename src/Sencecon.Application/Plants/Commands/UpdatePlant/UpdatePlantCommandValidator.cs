using FluentValidation;

namespace Sencecon.Application.Plants.Commands.UpdatePlant;

public class UpdatePlantCommandValidator : AbstractValidator<UpdatePlantCommand>
{
    public UpdatePlantCommandValidator()
    {
        RuleFor(v => v.Id)
            .NotEmpty();

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

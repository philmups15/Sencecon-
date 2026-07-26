using FluentValidation;

namespace Sencecon.Application.Surveys.Commands.CreateSurvey;

public class CreateSurveyCommandValidator : AbstractValidator<CreateSurveyCommand>
{
    public CreateSurveyCommandValidator()
    {
        RuleFor(v => v.Code)
            .NotEmpty()
            .MaximumLength(20);

        RuleFor(v => v.PlantName)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(v => v.Progress)
            .InclusiveBetween(0, 100);

        RuleFor(v => v.Surveyor)
            .MaximumLength(100);
    }
}

using FluentValidation;

namespace Sencecon.Application.NonConformities.Commands.CreateNonConformity;

public class CreateNonConformityCommandValidator : AbstractValidator<CreateNonConformityCommand>
{
    public CreateNonConformityCommandValidator()
    {
        RuleFor(v => v.Code)
            .NotEmpty()
            .MaximumLength(20);

        RuleFor(v => v.Description)
            .NotEmpty()
            .MaximumLength(500);

        RuleFor(v => v.PlantName)
            .MaximumLength(200);
    }
}

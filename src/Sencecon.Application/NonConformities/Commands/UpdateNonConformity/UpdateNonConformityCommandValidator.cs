using FluentValidation;

namespace Sencecon.Application.NonConformities.Commands.UpdateNonConformity;

public class UpdateNonConformityCommandValidator : AbstractValidator<UpdateNonConformityCommand>
{
    public UpdateNonConformityCommandValidator()
    {
        RuleFor(v => v.Id)
            .NotEmpty();

        RuleFor(v => v.Description)
            .NotEmpty()
            .MaximumLength(500);

        RuleFor(v => v.PlantName)
            .MaximumLength(200);
    }
}

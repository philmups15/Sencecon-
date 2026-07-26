using FluentValidation;

namespace Sencecon.Application.Designs.Commands.CreateDesign;

public class CreateDesignCommandValidator : AbstractValidator<CreateDesignCommand>
{
    public CreateDesignCommandValidator()
    {
        RuleFor(v => v.Code)
            .NotEmpty()
            .MaximumLength(20);

        RuleFor(v => v.ProjectName)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(v => v.Revision)
            .MaximumLength(10);
    }
}

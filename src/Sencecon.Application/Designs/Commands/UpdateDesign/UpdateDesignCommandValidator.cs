using FluentValidation;

namespace Sencecon.Application.Designs.Commands.UpdateDesign;

public class UpdateDesignCommandValidator : AbstractValidator<UpdateDesignCommand>
{
    public UpdateDesignCommandValidator()
    {
        RuleFor(v => v.Id)
            .NotEmpty();

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

using FluentValidation;

namespace Sencecon.Application.Projects.Commands.UpdateProject;

public class UpdateProjectCommandValidator : AbstractValidator<UpdateProjectCommand>
{
    public UpdateProjectCommandValidator()
    {
        RuleFor(v => v.Id)
            .NotEmpty();

        RuleFor(v => v.Code)
            .NotEmpty()
            .MaximumLength(20);

        RuleFor(v => v.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(v => v.Customer)
            .MaximumLength(200);

        RuleFor(v => v.ProjectManager)
            .MaximumLength(100);

        RuleFor(v => v.Budget)
            .GreaterThanOrEqualTo(0);

        RuleFor(v => v.Actual)
            .GreaterThanOrEqualTo(0);
    }
}

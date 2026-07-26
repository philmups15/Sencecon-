using FluentValidation;

namespace Sencecon.Application.Opportunities.Commands.CreateOpportunity;

public class CreateOpportunityCommandValidator : AbstractValidator<CreateOpportunityCommand>
{
    public CreateOpportunityCommandValidator()
    {
        RuleFor(v => v.Code)
            .NotEmpty()
            .MaximumLength(20);

        RuleFor(v => v.Customer)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(v => v.Capacity)
            .MaximumLength(50);

        RuleFor(v => v.Location)
            .MaximumLength(200);

        RuleFor(v => v.NextAction)
            .MaximumLength(200);

        RuleFor(v => v.Owner)
            .MaximumLength(100);

        RuleFor(v => v.Value)
            .GreaterThanOrEqualTo(0);
    }
}

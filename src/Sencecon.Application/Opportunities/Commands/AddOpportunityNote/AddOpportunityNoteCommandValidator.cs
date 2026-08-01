using FluentValidation;

namespace Sencecon.Application.Opportunities.Commands.AddOpportunityNote;

public class AddOpportunityNoteCommandValidator : AbstractValidator<AddOpportunityNoteCommand>
{
    public AddOpportunityNoteCommandValidator()
    {
        RuleFor(v => v.OpportunityId)
            .NotEmpty();

        RuleFor(v => v.Text)
            .NotEmpty()
            .MaximumLength(2000);
    }
}

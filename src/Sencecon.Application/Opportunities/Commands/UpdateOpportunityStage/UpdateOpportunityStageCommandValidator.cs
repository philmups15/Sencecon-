using FluentValidation;

namespace Sencecon.Application.Opportunities.Commands.UpdateOpportunityStage;

public class UpdateOpportunityStageCommandValidator : AbstractValidator<UpdateOpportunityStageCommand>
{
    public UpdateOpportunityStageCommandValidator()
    {
        RuleFor(v => v.Id)
            .NotEmpty();

        RuleFor(v => v.Stage)
            .IsInEnum();

        RuleFor(v => v.NextAction)
            .MaximumLength(200);

        RuleFor(v => v.SiteVisitNotes)
            .MaximumLength(1000);

        RuleFor(v => v.ProposalNotes)
            .MaximumLength(1000);

        RuleFor(v => v.NegotiationNotes)
            .MaximumLength(1000);
    }
}

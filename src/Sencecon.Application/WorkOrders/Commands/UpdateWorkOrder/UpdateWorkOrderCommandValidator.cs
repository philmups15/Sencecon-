using FluentValidation;

namespace Sencecon.Application.WorkOrders.Commands.UpdateWorkOrder;

public class UpdateWorkOrderCommandValidator : AbstractValidator<UpdateWorkOrderCommand>
{
    public UpdateWorkOrderCommandValidator()
    {
        RuleFor(v => v.Id)
            .NotEmpty();

        RuleFor(v => v.Code)
            .NotEmpty()
            .MaximumLength(20);

        RuleFor(v => v.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(v => v.Assignee)
            .MaximumLength(100);

        RuleFor(v => v.PlantId)
            .NotEmpty();
    }
}

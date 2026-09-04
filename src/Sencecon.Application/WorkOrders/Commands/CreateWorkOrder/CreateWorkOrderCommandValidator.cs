using FluentValidation;

namespace Sencecon.Application.WorkOrders.Commands.CreateWorkOrder;

public class CreateWorkOrderCommandValidator : AbstractValidator<CreateWorkOrderCommand>
{
    public CreateWorkOrderCommandValidator()
    {
        RuleFor(v => v.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(v => v.Assignee)
            .MaximumLength(100);

        RuleFor(v => v.PlantId)
            .NotEmpty();
    }
}

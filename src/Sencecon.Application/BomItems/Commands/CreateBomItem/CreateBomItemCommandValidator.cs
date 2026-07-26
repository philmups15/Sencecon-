using FluentValidation;

namespace Sencecon.Application.BomItems.Commands.CreateBomItem;

public class CreateBomItemCommandValidator : AbstractValidator<CreateBomItemCommand>
{
    public CreateBomItemCommandValidator()
    {
        RuleFor(v => v.Component)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(v => v.Quantity)
            .GreaterThan(0);

        RuleFor(v => v.UnitCost)
            .GreaterThanOrEqualTo(0);

        RuleFor(v => v.Supplier)
            .MaximumLength(200);
    }
}

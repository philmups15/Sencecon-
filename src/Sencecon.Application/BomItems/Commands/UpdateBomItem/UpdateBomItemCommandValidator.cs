using FluentValidation;

namespace Sencecon.Application.BomItems.Commands.UpdateBomItem;

public class UpdateBomItemCommandValidator : AbstractValidator<UpdateBomItemCommand>
{
    public UpdateBomItemCommandValidator()
    {
        RuleFor(v => v.Id)
            .NotEmpty();

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

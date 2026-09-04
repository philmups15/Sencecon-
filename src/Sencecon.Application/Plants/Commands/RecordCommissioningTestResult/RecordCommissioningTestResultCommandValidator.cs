using FluentValidation;

namespace Sencecon.Application.Plants.Commands.RecordCommissioningTestResult;

public class RecordCommissioningTestResultCommandValidator : AbstractValidator<RecordCommissioningTestResultCommand>
{
    public RecordCommissioningTestResultCommandValidator()
    {
        RuleFor(v => v.PlantId)
            .NotEmpty();

        RuleFor(v => v.Category)
            .IsInEnum();

        RuleFor(v => v.TestName)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(v => v.Result)
            .IsInEnum();

        RuleFor(v => v.Notes)
            .MaximumLength(1000);
    }
}

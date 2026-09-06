using FluentValidation;

namespace Sencecon.Application.Reports.Commands.CreateReport;

public class CreateReportCommandValidator : AbstractValidator<CreateReportCommand>
{
    public CreateReportCommandValidator()
    {
        RuleFor(v => v.Type)
            .IsInEnum();

        RuleFor(v => v.GeneratedBy)
            .MaximumLength(100);
    }
}

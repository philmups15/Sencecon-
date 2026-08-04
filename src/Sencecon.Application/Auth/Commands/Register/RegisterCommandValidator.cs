using FluentValidation;

namespace Sencecon.Application.Auth.Commands.Register;

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(v => v.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(256);

        RuleFor(v => v.Password)
            .NotEmpty()
            .MinimumLength(8)
            .MaximumLength(128);

        RuleFor(v => v.DisplayName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(v => v.Username).MaximumLength(100);
        RuleFor(v => v.PhoneNumber).MaximumLength(50);
        RuleFor(v => v.Address).MaximumLength(300);
        RuleFor(v => v.JobDescription).MaximumLength(200);
    }
}

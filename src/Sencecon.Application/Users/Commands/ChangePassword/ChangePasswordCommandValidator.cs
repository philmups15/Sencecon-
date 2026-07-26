using FluentValidation;

namespace Sencecon.Application.Users.Commands.ChangePassword;

public class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordCommandValidator()
    {
        RuleFor(v => v.CurrentPassword)
            .NotEmpty();

        RuleFor(v => v.NewPassword)
            .NotEmpty()
            .MinimumLength(8);
    }
}

using FluentValidation;

namespace Sencecon.Application.Users.Commands.UpdateProfile;

public class UpdateProfileCommandValidator : AbstractValidator<UpdateProfileCommand>
{
    public UpdateProfileCommandValidator()
    {
        RuleFor(v => v.DisplayName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(v => v.Username).MaximumLength(100);
        RuleFor(v => v.PhoneNumber).MaximumLength(50);
        RuleFor(v => v.Address).MaximumLength(300);
        RuleFor(v => v.JobDescription).MaximumLength(200);
    }
}

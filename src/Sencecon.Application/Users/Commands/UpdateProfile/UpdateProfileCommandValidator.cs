using FluentValidation;

namespace Sencecon.Application.Users.Commands.UpdateProfile;

public class UpdateProfileCommandValidator : AbstractValidator<UpdateProfileCommand>
{
    public UpdateProfileCommandValidator()
    {
        RuleFor(v => v.DisplayName)
            .NotEmpty()
            .MaximumLength(100);
    }
}

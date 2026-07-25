using FluentAssertions;
using Sencecon.Application.Auth.Commands.Register;
using Xunit;

namespace Sencecon.Application.UnitTests.Auth;

public class RegisterCommandValidatorTests
{
    private readonly RegisterCommandValidator _validator = new();

    [Fact]
    public void Should_Pass_When_Command_Is_Valid()
    {
        var command = new RegisterCommand
        {
            Email = "user@example.com",
            Password = "SuperSecret123",
            DisplayName = "Test User"
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Should_Fail_When_Email_Is_Invalid()
    {
        var command = new RegisterCommand
        {
            Email = "not-an-email",
            Password = "SuperSecret123",
            DisplayName = "Test User"
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(RegisterCommand.Email));
    }

    [Fact]
    public void Should_Fail_When_Password_Is_Too_Short()
    {
        var command = new RegisterCommand
        {
            Email = "user@example.com",
            Password = "short",
            DisplayName = "Test User"
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(RegisterCommand.Password));
    }
}

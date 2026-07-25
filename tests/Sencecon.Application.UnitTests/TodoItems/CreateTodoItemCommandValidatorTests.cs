using FluentAssertions;
using Sencecon.Application.TodoItems.Commands.CreateTodoItem;
using Xunit;

namespace Sencecon.Application.UnitTests.TodoItems;

public class CreateTodoItemCommandValidatorTests
{
    private readonly CreateTodoItemCommandValidator _validator = new();

    [Fact]
    public void Should_Pass_When_Command_Is_Valid()
    {
        var command = new CreateTodoItemCommand
        {
            Title = "Write documentation",
            Description = "Add README instructions",
            OwnerId = Guid.NewGuid()
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Should_Fail_When_Title_Is_Empty(string title)
    {
        var command = new CreateTodoItemCommand
        {
            Title = title,
            OwnerId = Guid.NewGuid()
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateTodoItemCommand.Title));
    }

    [Fact]
    public void Should_Fail_When_Title_Exceeds_Max_Length()
    {
        var command = new CreateTodoItemCommand
        {
            Title = new string('a', 201),
            OwnerId = Guid.NewGuid()
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
    }
}

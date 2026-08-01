using FluentAssertions;
using Sencecon.Application.Opportunities.Commands.UpdateOpportunityStage;
using Sencecon.Domain.Enums;
using Xunit;

namespace Sencecon.Application.UnitTests.Opportunities;

public class UpdateOpportunityStageCommandValidatorTests
{
    private readonly UpdateOpportunityStageCommandValidator _validator = new();

    [Fact]
    public void Should_Pass_When_Command_Is_Valid()
    {
        var command = new UpdateOpportunityStageCommand
        {
            Id = Guid.NewGuid(),
            Stage = OpportunityStage.SiteVisit,
            SiteVisitNotes = "Roof survey completed"
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Should_Fail_When_Id_Is_Empty()
    {
        var command = new UpdateOpportunityStageCommand
        {
            Id = Guid.Empty,
            Stage = OpportunityStage.Won
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateOpportunityStageCommand.Id));
    }

    [Fact]
    public void Should_Fail_When_Notes_Exceed_Max_Length()
    {
        var command = new UpdateOpportunityStageCommand
        {
            Id = Guid.NewGuid(),
            Stage = OpportunityStage.Negotiation,
            NegotiationNotes = new string('a', 1001)
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateOpportunityStageCommand.NegotiationNotes));
    }
}

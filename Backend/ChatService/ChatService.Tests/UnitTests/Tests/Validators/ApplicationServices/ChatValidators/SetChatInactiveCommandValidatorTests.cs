using ChatService.Application.UseCases.ChatUseCases.Commands.SetChatInactive;
using ChatService.Application.Validators.ChatValidators;
using FluentValidation.TestHelper;

namespace ChatService.Tests.UnitTests.Tests.Validators.ApplicationServices.ChatValidators;

public class SetChatInactiveCommandValidatorTests
{
    private readonly SetChatInactiveCommandValidator _validator = new();

    [Fact]
    public void Validate_WhenProjectIdIsValid_ShouldPass()
    {
        // Arrange
        var command = new SetChatInactiveCommand(ProjectId: Guid.NewGuid());

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WhenProjectIdIsEmpty_ShouldFail()
    {
        // Arrange
        var command = new SetChatInactiveCommand(ProjectId: Guid.Empty);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ProjectId)
            .WithErrorMessage("ChatId is required.");
    }
}
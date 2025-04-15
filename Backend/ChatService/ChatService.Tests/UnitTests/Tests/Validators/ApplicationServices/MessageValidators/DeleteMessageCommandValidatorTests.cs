using ChatService.Application.UseCases.MessageUseCases.Commands.DeleteMessage;
using ChatService.Application.Validators.MessageValidators;
using FluentValidation.TestHelper;

namespace ChatService.Tests.UnitTests.Tests.Validators.ApplicationServices.MessageValidators;

public class DeleteMessageCommandValidatorTests
{
    private readonly DeleteMessageCommandValidator _validator = new();

    [Fact]
    public void Validate_WhenMessageIdIsValid_ShouldPass()
    {
        // Arrange
        var command = new DeleteMessageCommand(MessageId: Guid.NewGuid());

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WhenMessageIdIsEmpty_ShouldFail()
    {
        // Arrange
        var command = new DeleteMessageCommand(MessageId: Guid.Empty);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.MessageId)
            .WithErrorMessage("MessageId is required.");
    }
}
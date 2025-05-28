using ChatService.Application.UseCases.MessageUseCases.Commands.CreateTextMessage;
using ChatService.Application.Validators.MessageValidators;
using FluentValidation.TestHelper;

namespace ChatService.Tests.UnitTests.Tests.Validators.ApplicationServices.MessageValidators;

public class CreateTextMessageCommandValidatorTests
{
    private readonly CreateTextMessageCommandValidator _validator = new();

    [Fact]
    public void Validate_WhenAllFieldsAreValid_ShouldPass()
    {
        // Arrange
        var command = new CreateTextMessageCommand(
            ChatId: Guid.NewGuid(),
            ReceiverId: Guid.NewGuid(),
            Text: "Hello, world!");

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WhenChatIdIsEmpty_ShouldFail()
    {
        // Arrange
        var command = new CreateTextMessageCommand(
            ChatId: Guid.Empty,
            ReceiverId: Guid.NewGuid(),
            Text: "Hello");

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ChatId)
            .WithErrorMessage("ChatId is required.");
    }

    [Fact]
    public void Validate_WhenReceiverIdIsEmpty_ShouldFail()
    {
        // Arrange
        var command = new CreateTextMessageCommand(
            ChatId: Guid.NewGuid(),
            ReceiverId: Guid.Empty,
            Text: "Hello");

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ReceiverId)
            .WithErrorMessage("ReceiverId is required.");
    }

    [Fact]
    public void Validate_WhenTextIsEmpty_ShouldFail()
    {
        // Arrange
        var command = new CreateTextMessageCommand(
            ChatId: Guid.NewGuid(),
            ReceiverId: Guid.NewGuid(),
            Text: string.Empty);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Text)
            .WithErrorMessage("Text is required.");
    }

    [Fact]
    public void Validate_WhenTextIsNull_ShouldFail()
    {
        // Arrange
        var command = new CreateTextMessageCommand(
            ChatId: Guid.NewGuid(),
            ReceiverId: Guid.NewGuid(),
            Text: null!);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Text)
            .WithErrorMessage("Text is required.");
    }

    [Fact]
    public void Validate_WhenTextExceedsMaxLength_ShouldFail()
    {
        // Arrange
        var longText = new string('a', 10_001);
        var command = new CreateTextMessageCommand(
            ChatId: Guid.NewGuid(),
            ReceiverId: Guid.NewGuid(),
            Text: longText);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Text)
            .WithErrorMessage("Text must not exceed 10000 characters.");
    }

    [Fact]
    public void Validate_WhenTextIsExactlyMaxLength_ShouldPass()
    {
        // Arrange
        var maxText = new string('a', 10_000);
        var command = new CreateTextMessageCommand(
            ChatId: Guid.NewGuid(),
            ReceiverId: Guid.NewGuid(),
            Text: maxText);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WhenAllFieldsAreInvalid_ShouldFailForAll()
    {
        // Arrange
        var command = new CreateTextMessageCommand(
            ChatId: Guid.Empty,
            ReceiverId: Guid.Empty,
            Text: string.Empty);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ChatId)
            .WithErrorMessage("ChatId is required.");
        result.ShouldHaveValidationErrorFor(x => x.ReceiverId)
            .WithErrorMessage("ReceiverId is required.");
        result.ShouldHaveValidationErrorFor(x => x.Text)
            .WithErrorMessage("Text is required.");
    }
}
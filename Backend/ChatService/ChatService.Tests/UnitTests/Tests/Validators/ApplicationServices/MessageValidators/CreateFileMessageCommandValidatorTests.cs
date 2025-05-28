using ChatService.Application.UseCases.MessageUseCases.Commands.CreateFileMessage;
using ChatService.Application.Validators.MessageValidators;
using FluentValidation.TestHelper;

namespace ChatService.Tests.UnitTests.Tests.Validators.ApplicationServices.MessageValidators;

public class CreateFileMessageCommandValidatorTests
{
    private readonly CreateFileMessageCommandValidator _validator = new();

    [Fact]
    public void Validate_WhenAllFieldsAreValid_ShouldPass()
    {
        // Arrange
        var command = new CreateFileMessageCommand(
            ChatId: Guid.NewGuid(),
            ReceiverId: Guid.NewGuid(),
            FileStream: new MemoryStream(new byte[] { 1, 2, 3 }),
            ContentType: "application/pdf");

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WhenChatIdIsEmpty_ShouldFail()
    {
        // Arrange
        var command = new CreateFileMessageCommand(
            ChatId: Guid.Empty,
            ReceiverId: Guid.NewGuid(),
            FileStream: new MemoryStream(),
            ContentType: "application/pdf");

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
        var command = new CreateFileMessageCommand(
            ChatId: Guid.NewGuid(),
            ReceiverId: Guid.Empty,
            FileStream: new MemoryStream(),
            ContentType: "application/pdf");

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ReceiverId)
            .WithErrorMessage("ReceiverId is required.");
    }

    [Fact]
    public void Validate_WhenFileStreamIsNull_ShouldFail()
    {
        // Arrange
        var command = new CreateFileMessageCommand(
            ChatId: Guid.NewGuid(),
            ReceiverId: Guid.NewGuid(),
            FileStream: null!,
            ContentType: "application/pdf");

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.FileStream)
            .WithErrorMessage("FileStream is required.");
    }

    [Fact]
    public void Validate_WhenContentTypeIsEmpty_ShouldFail()
    {
        // Arrange
        var command = new CreateFileMessageCommand(
            ChatId: Guid.NewGuid(),
            ReceiverId: Guid.NewGuid(),
            FileStream: new MemoryStream(),
            ContentType: string.Empty);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ContentType)
            .WithErrorMessage("ContentType is required.");
    }

    [Fact]
    public void Validate_WhenContentTypeIsNull_ShouldFail()
    {
        // Arrange
        var command = new CreateFileMessageCommand(
            ChatId: Guid.NewGuid(),
            ReceiverId: Guid.NewGuid(),
            FileStream: new MemoryStream(),
            ContentType: null!);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ContentType)
            .WithErrorMessage("ContentType is required.");
    }

    [Fact]
    public void Validate_WhenAllFieldsAreInvalid_ShouldFailForAll()
    {
        // Arrange
        var command = new CreateFileMessageCommand(
            ChatId: Guid.Empty,
            ReceiverId: Guid.Empty,
            FileStream: null!,
            ContentType: string.Empty);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ChatId)
            .WithErrorMessage("ChatId is required.");
        result.ShouldHaveValidationErrorFor(x => x.ReceiverId)
            .WithErrorMessage("ReceiverId is required.");
        result.ShouldHaveValidationErrorFor(x => x.FileStream)
            .WithErrorMessage("FileStream is required.");
        result.ShouldHaveValidationErrorFor(x => x.ContentType)
            .WithErrorMessage("ContentType is required.");
    }
}
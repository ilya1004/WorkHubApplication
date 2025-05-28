using ChatService.Application.UseCases.MessageUseCases.Queries.GetChatMessages;
using ChatService.Application.Validators.MessageValidators;
using FluentValidation.TestHelper;

namespace ChatService.Tests.UnitTests.Tests.Validators.ApplicationServices.MessageValidators;

public class GetChatMessagesQueryValidatorTests
{
    private readonly GetChatMessagesQueryValidator _validator = new();

    [Theory]
    [InlineData(1, 10)]
    [InlineData(100, 100)]
    [InlineData(100_000, 1000)]
    public void Validate_WhenAllFieldsAreValid_ShouldPass(int pageNo, int pageSize)
    {
        // Arrange
        var query = new GetChatMessagesQuery(ChatId: Guid.NewGuid(), PageNo: pageNo, PageSize: pageSize);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WhenChatIdIsEmpty_ShouldFail()
    {
        // Arrange
        var query = new GetChatMessagesQuery(ChatId: Guid.Empty, PageNo: 1, PageSize: 10);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ChatId)
            .WithErrorMessage("ChatId is required.");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_WhenPageNoIsLessThanOne_ShouldFail(int pageNo)
    {
        // Arrange
        var query = new GetChatMessagesQuery(ChatId: Guid.NewGuid(), PageNo: pageNo, PageSize: 10);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PageNo)
            .WithErrorMessage("Page number must be between 1 and 100_000.");
    }

    [Fact]
    public void Validate_WhenPageNoIsGreaterThanMax_ShouldFail()
    {
        // Arrange
        var query = new GetChatMessagesQuery(ChatId: Guid.NewGuid(), PageNo: 100_001, PageSize: 10);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PageNo)
            .WithErrorMessage("Page number must be between 1 and 100_000.");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_WhenPageSizeIsLessThanOne_ShouldFail(int pageSize)
    {
        // Arrange
        var query = new GetChatMessagesQuery(ChatId: Guid.NewGuid(), PageNo: 1, PageSize: pageSize);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PageSize)
            .WithErrorMessage("Page size must be between 1 and 1000.");
    }

    [Fact]
    public void Validate_WhenPageSizeIsGreaterThanMax_ShouldFail()
    {
        // Arrange
        var query = new GetChatMessagesQuery(ChatId: Guid.NewGuid(), PageNo: 1, PageSize: 1001);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PageSize)
            .WithErrorMessage("Page size must be between 1 and 1000.");
    }

    [Fact]
    public void Validate_WhenAllFieldsAreInvalid_ShouldFailForAll()
    {
        // Arrange
        var query = new GetChatMessagesQuery(ChatId: Guid.Empty, PageNo: 0, PageSize: 0);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ChatId)
            .WithErrorMessage("ChatId is required.");
        result.ShouldHaveValidationErrorFor(x => x.PageNo)
            .WithErrorMessage("Page number must be between 1 and 100_000.");
        result.ShouldHaveValidationErrorFor(x => x.PageSize)
            .WithErrorMessage("Page size must be between 1 and 1000.");
    }
}
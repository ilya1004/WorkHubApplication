using ChatService.Application.UseCases.ChatUseCases.Queries.GetAllChats;
using ChatService.Application.Validators.ChatValidators;
using FluentValidation.TestHelper;

namespace ChatService.Tests.UnitTests.Tests.Validators.ApplicationServices.ChatValidators;

public class GetAllChatsQueryValidatorTests
{
    private readonly GetAllChatsQueryValidator _validator = new();

    [Theory]
    [InlineData(1, 10)]
    [InlineData(100, 100)]
    [InlineData(100_000, 1000)]
    public void Validate_WhenPageNoAndPageSizeAreValid_ShouldPass(int pageNo, int pageSize)
    {
        // Arrange
        var query = new GetAllChatsQuery(PageNo: pageNo, PageSize: pageSize);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_WhenPageNoIsLessThanOne_ShouldFail(int pageNo)
    {
        // Arrange
        var query = new GetAllChatsQuery(PageNo: pageNo, PageSize: 10);

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
        var query = new GetAllChatsQuery(PageNo: 100_001, PageSize: 10);

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
        var query = new GetAllChatsQuery(PageNo: 1, PageSize: pageSize);

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
        var query = new GetAllChatsQuery(PageNo: 1, PageSize: 1001);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PageSize)
            .WithErrorMessage("Page size must be between 1 and 1000.");
    }

    [Fact]
    public void Validate_WhenBothPageNoAndPageSizeAreInvalid_ShouldFailForBoth()
    {
        // Arrange
        var query = new GetAllChatsQuery(PageNo: 0, PageSize: 0);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PageNo)
            .WithErrorMessage("Page number must be between 1 and 100_000.");
        result.ShouldHaveValidationErrorFor(x => x.PageSize)
            .WithErrorMessage("Page size must be between 1 and 1000.");
    }
}
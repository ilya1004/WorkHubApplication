using FluentValidation.TestHelper;
using IdentityService.API.Contracts.CommonContracts;
using IdentityService.API.Validators.CommonValidators;

namespace IdentityService.Tests.UnitTests.Tests.Validators.ApiValidators.CommonValidators;

public class GetPaginatedListRequestValidatorTests
{
    private readonly GetPaginatedListRequestValidator _validator = new();

    [Fact]
    public void Should_Pass_When_ValidRequest()
    {
        // Arrange
        var request = new GetPaginatedListRequest(5, 50);

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_Pass_When_DefaultValues()
    {
        // Arrange
        var request = new GetPaginatedListRequest();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_Fail_When_PageNoIsLessThan1()
    {
        // Arrange
        var request = new GetPaginatedListRequest(0, 50);

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PageNo)
            .WithErrorMessage("Page number must be between 1 and 100_000.");
    }

    [Fact]
    public void Should_Fail_When_PageNoIsGreaterThan100000()
    {
        // Arrange
        var request = new GetPaginatedListRequest(100_001, 50);

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PageNo)
            .WithErrorMessage("Page number must be between 1 and 100_000.");
    }

    [Fact]
    public void Should_Fail_When_PageSizeIsLessThan1()
    {
        // Arrange
        var request = new GetPaginatedListRequest(5, 0);

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PageSize)
            .WithErrorMessage("Page size must be between 1 and 1000.");
    }

    [Fact]
    public void Should_Fail_When_PageSizeIsGreaterThan1000()
    {
        // Arrange
        var request = new GetPaginatedListRequest(5, 1001);

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PageSize)
            .WithErrorMessage("Page size must be between 1 and 1000.");
    }
}
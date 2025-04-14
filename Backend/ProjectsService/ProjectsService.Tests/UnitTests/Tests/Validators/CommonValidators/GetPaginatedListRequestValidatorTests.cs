using FluentValidation.TestHelper;
using ProjectsService.API.Contracts.CommonContracts;
using ProjectsService.API.Validators.CommonValidators;

namespace ProjectsService.Tests.UnitTests.Tests.Validators.CommonValidators;

public class GetPaginatedListRequestValidatorTests
{
    private readonly GetPaginatedListRequestValidator _validator = new();

    [Fact]
    public void Validate_ValidPagination_Succeeds()
    {
        // Arrange
        var request = new GetPaginatedListRequest(PageNo: 5, PageSize: 50);

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_DefaultPagination_Succeeds()
    {
        // Arrange
        var request = new GetPaginatedListRequest();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_PageNoLessThan1_FailsWithCorrectMessage()
    {
        // Arrange
        var request = new GetPaginatedListRequest(PageNo: 0, PageSize: 10);

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PageNo)
            .WithErrorMessage("Page number must be between 1 and 100_000.");
    }

    [Fact]
    public void Validate_PageNoGreaterThan100000_FailsWithCorrectMessage()
    {
        // Arrange
        var request = new GetPaginatedListRequest(PageNo: 100_001, PageSize: 10);

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PageNo)
            .WithErrorMessage("Page number must be between 1 and 100_000.");
    }

    [Fact]
    public void Validate_PageSizeLessThan1_FailsWithCorrectMessage()
    {
        // Arrange
        var request = new GetPaginatedListRequest(PageNo: 1, PageSize: 0);

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PageSize)
            .WithErrorMessage("Page size must be between 1 and 1000.");
    }

    [Fact]
    public void Validate_PageSizeGreaterThan1000_FailsWithCorrectMessage()
    {
        // Arrange
        var request = new GetPaginatedListRequest(PageNo: 1, PageSize: 1001);

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PageSize)
            .WithErrorMessage("Page size must be between 1 and 1000.");
    }
}
using FluentValidation.TestHelper;
using ProjectsService.API.Contracts.ProjectContracts;
using ProjectsService.API.Validators.ProjectValidators;

namespace ProjectsService.Tests.UnitTests.Tests.Validators.ProjectValidators;

public class GetProjectsByFilterRequestValidatorTests
{
    private readonly GetProjectsByFilterRequestValidator _validator = new();

    [Fact]
    public void Validate_ValidRequest_Succeeds()
    {
        // Arrange
        var request = new GetProjectsByFilterRequest(
            Title: "Test Project",
            BudgetFrom: 1000.50m,
            BudgetTo: 2000.75m,
            CategoryId: Guid.NewGuid(),
            EmployerId: Guid.NewGuid(),
            ProjectStatus: ProjectStatus.InProgress,
            PageNo: 5,
            PageSize: 50);

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_NullValues_Succeeds()
    {
        // Arrange
        var request = new GetProjectsByFilterRequest(
            Title: null,
            BudgetFrom: null,
            BudgetTo: null,
            CategoryId: null,
            EmployerId: null,
            ProjectStatus: null,
            PageNo: 1,
            PageSize: 10);

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_TitleExceeds200Characters_FailsWithCorrectMessage()
    {
        // Arrange
        var request = new GetProjectsByFilterRequest(
            Title: new string('A', 201),
            BudgetFrom: null,
            BudgetTo: null,
            CategoryId: null,
            EmployerId: null,
            ProjectStatus: null,
            PageNo: 1,
            PageSize: 10);

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Title)
            .WithErrorMessage("Title must not exceed 200 characters.");
    }

    [Fact]
    public void Validate_NegativeBudgetFrom_FailsWithCorrectMessage()
    {
        // Arrange
        var request = new GetProjectsByFilterRequest(
            Title: null,
            BudgetFrom: -100m,
            BudgetTo: null,
            CategoryId: null,
            EmployerId: null,
            ProjectStatus: null,
            PageNo: 1,
            PageSize: 10);

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.BudgetFrom)
            .WithErrorMessage("BudgetFrom must be greater than or equal to zero.");
    }

    [Fact]
    public void Validate_NegativeBudgetTo_FailsWithCorrectMessage()
    {
        // Arrange
        var request = new GetProjectsByFilterRequest(
            Title: null,
            BudgetFrom: 100m,
            BudgetTo: -100m,
            CategoryId: null,
            EmployerId: null,
            ProjectStatus: null,
            PageNo: 1,
            PageSize: 10);

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.BudgetTo)
            .WithErrorMessage("BudgetTo must be greater than or equal to zero.");
    }

    [Fact]
    public void Validate_BudgetToLessThanBudgetFrom_FailsWithCorrectMessage()
    {
        // Arrange
        var request = new GetProjectsByFilterRequest(
            Title: null,
            BudgetFrom: 2000m,
            BudgetTo: 1000m,
            CategoryId: null,
            EmployerId: null,
            ProjectStatus: null,
            PageNo: 1,
            PageSize: 10);

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.BudgetTo)
            .WithErrorMessage("BudgetTo must be greater than BudgetFrom.");
    }

    [Fact]
    public void Validate_InvalidProjectStatus_FailsWithCorrectMessage()
    {
        // Arrange
        var request = new GetProjectsByFilterRequest(
            Title: null,
            BudgetFrom: null,
            BudgetTo: null,
            CategoryId: null,
            EmployerId: null,
            ProjectStatus: (ProjectStatus)999,
            PageNo: 1,
            PageSize: 10);

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ProjectStatus)
            .WithErrorMessage("ProjectStatus must be a valid Enum value.");
    }

    [Fact]
    public void Validate_PageNoLessThan1_FailsWithCorrectMessage()
    {
        // Arrange
        var request = new GetProjectsByFilterRequest(
            Title: null,
            BudgetFrom: null,
            BudgetTo: null,
            CategoryId: null,
            EmployerId: null,
            ProjectStatus: null,
            PageNo: 0,
            PageSize: 10);

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
        var request = new GetProjectsByFilterRequest(
            Title: null,
            BudgetFrom: null,
            BudgetTo: null,
            CategoryId: null,
            EmployerId: null,
            ProjectStatus: null,
            PageNo: 100_001,
            PageSize: 10);

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
        var request = new GetProjectsByFilterRequest(
            Title: null,
            BudgetFrom: null,
            BudgetTo: null,
            CategoryId: null,
            EmployerId: null,
            ProjectStatus: null,
            PageNo: 1,
            PageSize: 0);

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
        var request = new GetProjectsByFilterRequest(
            Title: null,
            BudgetFrom: null,
            BudgetTo: null,
            CategoryId: null,
            EmployerId: null,
            ProjectStatus: null,
            PageNo: 1,
            PageSize: 1001);

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PageSize)
            .WithErrorMessage("Page size must be between 1 and 1000.");
    }
}
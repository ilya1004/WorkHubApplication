using FluentValidation.TestHelper;
using ProjectsService.API.Contracts.ProjectContracts;
using ProjectsService.API.Validators.ProjectValidators;

namespace ProjectsService.Tests.UnitTests.Tests.Validators.ProjectValidators;

public class GetProjectsByEmployerFilterRequestValidatorTests
{
    private readonly GetProjectsByEmployerFilterRequestValidator _validator = new();

    [Fact]
    public void Validate_ValidRequest_Succeeds()
    {
        // Arrange
        var request = new GetProjectsByEmployerFilterRequest(
            UpdatedAtStartDate: DateTime.UtcNow.AddDays(-10),
            UpdatedAtEndDate: DateTime.UtcNow.AddDays(-1),
            ProjectStatus: ProjectStatus.InProgress,
            AcceptanceRequestedAndNotConfirmed: true,
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
        var request = new GetProjectsByEmployerFilterRequest(
            UpdatedAtStartDate: null,
            UpdatedAtEndDate: null,
            ProjectStatus: null,
            AcceptanceRequestedAndNotConfirmed: null,
            PageNo: 1,
            PageSize: 10);

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_UpdatedAtStartDateInFuture_FailsWithCorrectMessage()
    {
        // Arrange
        var request = new GetProjectsByEmployerFilterRequest(
            UpdatedAtStartDate: DateTime.UtcNow.AddDays(1),
            UpdatedAtEndDate: null,
            ProjectStatus: null,
            AcceptanceRequestedAndNotConfirmed: null,
            PageNo: 1,
            PageSize: 10);

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.UpdatedAtStartDate)
            .WithErrorMessage("UpdatedAtStartDate must be in the past.");
    }

    [Fact]
    public void Validate_UpdatedAtEndDateBeforeStartDate_FailsWithCorrectMessage()
    {
        // Arrange
        var request = new GetProjectsByEmployerFilterRequest(
            UpdatedAtStartDate: DateTime.UtcNow.AddDays(-5),
            UpdatedAtEndDate: DateTime.UtcNow.AddDays(-10),
            ProjectStatus: null,
            AcceptanceRequestedAndNotConfirmed: null,
            PageNo: 1,
            PageSize: 10);

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.UpdatedAtEndDate)
            .WithErrorMessage("UpdatedAtEndDate must be after the UpdatedAtStartDate.");
    }

    [Fact]
    public void Validate_InvalidProjectStatus_FailsWithCorrectMessage()
    {
        // Arrange
        var request = new GetProjectsByEmployerFilterRequest(
            UpdatedAtStartDate: null,
            UpdatedAtEndDate: null,
            ProjectStatus: (ProjectStatus)999,
            AcceptanceRequestedAndNotConfirmed: null,
            PageNo: 1,
            PageSize: 10);

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ProjectStatus)
            .WithErrorMessage("ProjectStatus must be a valid Enum value.");
    }

    [Fact]
    public void Validate_AcceptanceRequestedAndNotConfirmedFalse_FailsWithCorrectMessage()
    {
        // Arrange
        var request = new GetProjectsByEmployerFilterRequest(
            UpdatedAtStartDate: null,
            UpdatedAtEndDate: null,
            ProjectStatus: null,
            AcceptanceRequestedAndNotConfirmed: false,
            PageNo: 1,
            PageSize: 10);

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.AcceptanceRequestedAndNotConfirmed)
            .WithErrorMessage("AcceptanceRequestedAndNotConfirmed value must be true or null.");
    }

    [Fact]
    public void Validate_PageNoLessThan1_FailsWithCorrectMessage()
    {
        // Arrange
        var request = new GetProjectsByEmployerFilterRequest(
            UpdatedAtStartDate: null,
            UpdatedAtEndDate: null,
            ProjectStatus: null,
            AcceptanceRequestedAndNotConfirmed: null,
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
        var request = new GetProjectsByEmployerFilterRequest(
            UpdatedAtStartDate: null,
            UpdatedAtEndDate: null,
            ProjectStatus: null,
            AcceptanceRequestedAndNotConfirmed: null,
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
        var request = new GetProjectsByEmployerFilterRequest(
            UpdatedAtStartDate: null,
            UpdatedAtEndDate: null,
            ProjectStatus: null,
            AcceptanceRequestedAndNotConfirmed: null,
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
        var request = new GetProjectsByEmployerFilterRequest(
            UpdatedAtStartDate: null,
            UpdatedAtEndDate: null,
            ProjectStatus: null,
            AcceptanceRequestedAndNotConfirmed: null,
            PageNo: 1,
            PageSize: 1001);

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PageSize)
            .WithErrorMessage("Page size must be between 1 and 1000.");
    }
}
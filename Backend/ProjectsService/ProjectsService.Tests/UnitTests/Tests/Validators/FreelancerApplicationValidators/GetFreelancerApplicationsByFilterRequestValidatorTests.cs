using FluentValidation.TestHelper;
using ProjectsService.API.Contracts.FreelancerApplicationContracts;
using ProjectsService.API.Validators.FreelancerApplicationValidators;

namespace ProjectsService.Tests.UnitTests.Tests.Validators.FreelancerApplicationValidators;

public class GetFreelancerApplicationsByFilterRequestValidatorTests
{
    private readonly GetFreelancerApplicationsByFilterRequestValidator _validator = new();

    [Fact]
    public void Validate_ValidRequest_Succeeds()
    {
        // Arrange
        var request = new GetMyFreelancerApplicationsByFilterRequest(
            StartDate: new DateTime(2025, 1, 1),
            EndDate: new DateTime(2025, 12, 31),
            ApplicationStatus: ApplicationStatus.Accepted,
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
        var request = new GetMyFreelancerApplicationsByFilterRequest(
            StartDate: null,
            EndDate: null,
            ApplicationStatus: null,
            PageNo: 1,
            PageSize: 10);

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_StartDateAfterEndDate_FailsWithCorrectMessage()
    {
        // Arrange
        var request = new GetMyFreelancerApplicationsByFilterRequest(
            StartDate: new DateTime(2025, 12, 31),
            EndDate: new DateTime(2025, 1, 1),
            ApplicationStatus: null,
            PageNo: 1,
            PageSize: 10);

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.StartDate)
            .WithErrorMessage("Start date must be before or equal to end date.");
    }

    [Fact]
    public void Validate_EndDateBeforeStartDate_FailsWithCorrectMessage()
    {
        // Arrange
        var request = new GetMyFreelancerApplicationsByFilterRequest(
            StartDate: new DateTime(2025, 12, 31),
            EndDate: new DateTime(2025, 1, 1),
            ApplicationStatus: null,
            PageNo: 1,
            PageSize: 10);

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.EndDate)
            .WithErrorMessage("End date must be after or equal to start date.");
    }

    [Fact]
    public void Validate_InvalidApplicationStatus_FailsWithCorrectMessage()
    {
        // Arrange
        var request = new GetMyFreelancerApplicationsByFilterRequest(
            StartDate: null,
            EndDate: null,
            ApplicationStatus: (ApplicationStatus)999,
            PageNo: 1,
            PageSize: 10);

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ApplicationStatus)
            .WithErrorMessage("Invalid application status.");
    }

    [Fact]
    public void Validate_PageNoLessThan1_FailsWithCorrectMessage()
    {
        // Arrange
        var request = new GetMyFreelancerApplicationsByFilterRequest(
            StartDate: null,
            EndDate: null,
            ApplicationStatus: null,
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
        var request = new GetMyFreelancerApplicationsByFilterRequest(
            StartDate: null,
            EndDate: null,
            ApplicationStatus: null,
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
        var request = new GetMyFreelancerApplicationsByFilterRequest(
            StartDate: null,
            EndDate: null,
            ApplicationStatus: null,
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
        var request = new GetMyFreelancerApplicationsByFilterRequest(
            StartDate: null,
            EndDate: null,
            ApplicationStatus: null,
            PageNo: 1,
            PageSize: 1001);

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PageSize)
            .WithErrorMessage("Page size must be between 1 and 1000.");
    }
}
using FluentValidation.TestHelper;
using ProjectsService.API.Contracts.ProjectContracts;
using ProjectsService.API.Validators.ProjectValidators;

namespace ProjectsService.Tests.UnitTests.Tests.Validators.ProjectValidators;

public class GetProjectsByFreelancerFilterRequestValidatorTests
{
    private readonly GetProjectsByFreelancerFilterRequestValidator _validator = new();

    [Fact]
    public void Validate_ValidRequest_Succeeds()
    {
        // Arrange
        var request = new GetProjectsByFreelancerFilterRequest(
            ProjectStatus: ProjectStatus.Completed,
            EmployerId: Guid.NewGuid(),
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
        var request = new GetProjectsByFreelancerFilterRequest(
            ProjectStatus: null,
            EmployerId: null,
            PageNo: 1,
            PageSize: 10);

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_InvalidProjectStatus_FailsWithCorrectMessage()
    {
        // Arrange
        var request = new GetProjectsByFreelancerFilterRequest(
            ProjectStatus: (ProjectStatus)999,
            EmployerId: null,
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
        var request = new GetProjectsByFreelancerFilterRequest(
            ProjectStatus: null,
            EmployerId: null,
            PageNo: 0,
            PageSize: 10);

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PageNo)
            .WithErrorMessage("Page number must be between 1 and 100_000.");
    }
}
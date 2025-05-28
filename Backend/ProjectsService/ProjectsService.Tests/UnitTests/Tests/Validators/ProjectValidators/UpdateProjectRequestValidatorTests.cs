using FluentValidation.TestHelper;
using ProjectsService.API.Contracts.ProjectContracts;
using ProjectsService.API.Validators.ProjectValidators;

namespace ProjectsService.Tests.UnitTests.Tests.Validators.ProjectValidators;

public class UpdateProjectRequestValidatorTests
{
    private readonly UpdateProjectRequestValidator _validator = new();

    [Fact]
    public void Validate_ValidRequest_Succeeds()
    {
        // Arrange
        var request = new UpdateProjectRequest(
            Project: new UpdateProjectDto(
                Title: "Updated Project",
                Description: "Updated Description",
                Budget: 2000.75m,
                CategoryId: Guid.NewGuid()),
            Lifecycle: new LifecycleDto(
                ApplicationsStartDate: DateTime.UtcNow.AddDays(1),
                ApplicationsDeadline: DateTime.UtcNow.AddDays(2),
                WorkStartDate: DateTime.UtcNow.AddDays(3),
                WorkDeadline: DateTime.UtcNow.AddDays(4)));

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_EmptyTitle_FailsWithCorrectMessage()
    {
        // Arrange
        var request = new UpdateProjectRequest(
            Project: new UpdateProjectDto(
                Title: "",
                Description: "Updated Description",
                Budget: 2000.75m,
                CategoryId: Guid.NewGuid()),
            Lifecycle: new LifecycleDto(
                ApplicationsStartDate: DateTime.UtcNow.AddDays(1),
                ApplicationsDeadline: DateTime.UtcNow.AddDays(2),
                WorkStartDate: DateTime.UtcNow.AddDays(3),
                WorkDeadline: DateTime.UtcNow.AddDays(4)));

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Project.Title)
            .WithErrorMessage("Title is required.");
    }

    [Fact]
    public void Validate_TitleExceeds200Characters_FailsWithCorrectMessage()
    {
        // Arrange
        var request = new UpdateProjectRequest(
            Project: new UpdateProjectDto(
                Title: new string('A', 201),
                Description: "Updated Description",
                Budget: 2000.75m,
                CategoryId: Guid.NewGuid()),
            Lifecycle: new LifecycleDto(
                ApplicationsStartDate: DateTime.UtcNow.AddDays(1),
                ApplicationsDeadline: DateTime.UtcNow.AddDays(2),
                WorkStartDate: DateTime.UtcNow.AddDays(3),
                WorkDeadline: DateTime.UtcNow.AddDays(4)));

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Project.Title)
            .WithErrorMessage("Title must not exceed 200 characters.");
    }

    [Fact]
    public void Validate_DescriptionExceeds1000Characters_FailsWithCorrectMessage()
    {
        // Arrange
        var request = new UpdateProjectRequest(
            Project: new UpdateProjectDto(
                Title: "Updated Project",
                Description: new string('A', 1001),
                Budget: 2000.75m,
                CategoryId: Guid.NewGuid()),
            Lifecycle: new LifecycleDto(
                ApplicationsStartDate: DateTime.UtcNow.AddDays(1),
                ApplicationsDeadline: DateTime.UtcNow.AddDays(2),
                WorkStartDate: DateTime.UtcNow.AddDays(3),
                WorkDeadline: DateTime.UtcNow.AddDays(4)));

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Project.Description)
            .WithErrorMessage("Description must not exceed 1000 characters.");
    }

    [Fact]
    public void Validate_ZeroBudget_FailsWithCorrectMessage()
    {
        // Arrange
        var request = new UpdateProjectRequest(
            Project: new UpdateProjectDto(
                Title: "Updated Project",
                Description: "Updated Description",
                Budget: 0m,
                CategoryId: Guid.NewGuid()),
            Lifecycle: new LifecycleDto(
                ApplicationsStartDate: DateTime.UtcNow.AddDays(1),
                ApplicationsDeadline: DateTime.UtcNow.AddDays(2),
                WorkStartDate: DateTime.UtcNow.AddDays(3),
                WorkDeadline: DateTime.UtcNow.AddDays(4)));

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Project.Budget)
            .WithErrorMessage("Budget must be greater than zero.");
    }

    [Fact]
    public void Validate_BudgetInvalidPrecision_FailsWithCorrectMessage()
    {
        // Arrange
        var request = new UpdateProjectRequest(
            Project: new UpdateProjectDto(
                Title: "Updated Project",
                Description: "Updated Description",
                Budget: 123456789123456789.123m,
                CategoryId: Guid.NewGuid()),
            Lifecycle: new LifecycleDto(
                ApplicationsStartDate: DateTime.UtcNow.AddDays(1),
                ApplicationsDeadline: DateTime.UtcNow.AddDays(2),
                WorkStartDate: DateTime.UtcNow.AddDays(3),
                WorkDeadline: DateTime.UtcNow.AddDays(4)));

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Project.Budget)
            .WithErrorMessage("Budget must have up to 18 digits and 2 decimal places.");
    }

    [Fact]
    public void Validate_NullCategoryId_Succeeds()
    {
        // Arrange
        var request = new UpdateProjectRequest(
            Project: new UpdateProjectDto(
                Title: "Updated Project",
                Description: "Updated Description",
                Budget: 2000.75m,
                CategoryId: null),
            Lifecycle: new LifecycleDto(
                ApplicationsStartDate: DateTime.UtcNow.AddDays(1),
                ApplicationsDeadline: DateTime.UtcNow.AddDays(2),
                WorkStartDate: DateTime.UtcNow.AddDays(3),
                WorkDeadline: DateTime.UtcNow.AddDays(4)));

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_ApplicationsStartDateInPast_FailsWithCorrectMessage()
    {
        // Arrange
        var request = new UpdateProjectRequest(
            Project: new UpdateProjectDto(
                Title: "Updated Project",
                Description: "Updated Description",
                Budget: 2000.75m,
                CategoryId: Guid.NewGuid()),
            Lifecycle: new LifecycleDto(
                ApplicationsStartDate: DateTime.UtcNow.AddDays(-1),
                ApplicationsDeadline: DateTime.UtcNow.AddDays(2),
                WorkStartDate: DateTime.UtcNow.AddDays(3),
                WorkDeadline: DateTime.UtcNow.AddDays(4)));

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Lifecycle.ApplicationsStartDate)
            .WithErrorMessage("Applications start date must be in the future.");
    }

    [Fact]
    public void Validate_ApplicationsDeadlineBeforeStartDate_FailsWithCorrectMessage()
    {
        // Arrange
        var request = new UpdateProjectRequest(
            Project: new UpdateProjectDto(
                Title: "Updated Project",
                Description: "Updated Description",
                Budget: 2000.75m,
                CategoryId: Guid.NewGuid()),
            Lifecycle: new LifecycleDto(
                ApplicationsStartDate: DateTime.UtcNow.AddDays(2),
                ApplicationsDeadline: DateTime.UtcNow.AddDays(1),
                WorkStartDate: DateTime.UtcNow.AddDays(3),
                WorkDeadline: DateTime.UtcNow.AddDays(4)));

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Lifecycle.ApplicationsDeadline)
            .WithErrorMessage("Applications deadline must be after the applications start date.");
    }

    [Fact]
    public void Validate_WorkStartDateBeforeApplicationsDeadline_FailsWithCorrectMessage()
    {
        // Arrange
        var request = new UpdateProjectRequest(
            Project: new UpdateProjectDto(
                Title: "Updated Project",
                Description: "Updated Description",
                Budget: 2000.75m,
                CategoryId: Guid.NewGuid()),
            Lifecycle: new LifecycleDto(
                ApplicationsStartDate: DateTime.UtcNow.AddDays(1),
                ApplicationsDeadline: DateTime.UtcNow.AddDays(3),
                WorkStartDate: DateTime.UtcNow.AddDays(2),
                WorkDeadline: DateTime.UtcNow.AddDays(4)));

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Lifecycle.WorkStartDate)
            .WithErrorMessage("Work start date must be after the applications deadline.");
    }

    [Fact]
    public void Validate_WorkDeadlineBeforeWorkStartDate_FailsWithCorrectMessage()
    {
        // Arrange
        var request = new UpdateProjectRequest(
            Project: new UpdateProjectDto(
                Title: "Updated Project",
                Description: "Updated Description",
                Budget: 2000.75m,
                CategoryId: Guid.NewGuid()),
            Lifecycle: new LifecycleDto(
                ApplicationsStartDate: DateTime.UtcNow.AddDays(1),
                ApplicationsDeadline: DateTime.UtcNow.AddDays(2),
                WorkStartDate: DateTime.UtcNow.AddDays(4),
                WorkDeadline: DateTime.UtcNow.AddDays(3)));

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Lifecycle.WorkDeadline)
            .WithErrorMessage("Work deadline must be after the work start date.");
    }
}
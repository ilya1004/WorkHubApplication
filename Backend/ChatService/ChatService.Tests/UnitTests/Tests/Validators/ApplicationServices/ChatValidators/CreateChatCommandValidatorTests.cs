using ChatService.Application.UseCases.ChatUseCases.Commands.CreateChat;
using ChatService.Application.Validators.ChatValidators;
using FluentValidation.TestHelper;

namespace ChatService.Tests.UnitTests.Tests.Validators.ApplicationServices.ChatValidators;

public class CreateChatCommandValidatorTests
{
    private readonly CreateChatCommandValidator _validator = new();

    [Fact]
    public void Validate_WhenAllIdsAreValid_ShouldPass()
    {
        // Arrange
        var command = new CreateChatCommand(
            EmployerId: Guid.NewGuid(),
            FreelancerId: Guid.NewGuid(),
            ProjectId: Guid.NewGuid());

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WhenEmployerIdIsEmpty_ShouldFail()
    {
        // Arrange
        var command = new CreateChatCommand(
            EmployerId: Guid.Empty,
            FreelancerId: Guid.NewGuid(),
            ProjectId: Guid.NewGuid());

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.EmployerId)
            .WithErrorMessage("EmployerId is required.");
    }

    [Fact]
    public void Validate_WhenFreelancerIdIsEmpty_ShouldFail()
    {
        // Arrange
        var command = new CreateChatCommand(
            EmployerId: Guid.NewGuid(),
            FreelancerId: Guid.Empty,
            ProjectId: Guid.NewGuid());

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.FreelancerId)
            .WithErrorMessage("FreelancerId is required.");
    }

    [Fact]
    public void Validate_WhenProjectIdIsEmpty_ShouldFail()
    {
        // Arrange
        var command = new CreateChatCommand(
            EmployerId: Guid.NewGuid(),
            FreelancerId: Guid.NewGuid(),
            ProjectId: Guid.Empty);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ProjectId)
            .WithErrorMessage("ProjectId is required.");
    }

    [Fact]
    public void Validate_WhenAllIdsAreEmpty_ShouldFailForAll()
    {
        // Arrange
        var command = new CreateChatCommand(
            EmployerId: Guid.Empty,
            FreelancerId: Guid.Empty,
            ProjectId: Guid.Empty);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.EmployerId)
            .WithErrorMessage("EmployerId is required.");
        result.ShouldHaveValidationErrorFor(x => x.FreelancerId)
            .WithErrorMessage("FreelancerId is required.");
        result.ShouldHaveValidationErrorFor(x => x.ProjectId)
            .WithErrorMessage("ProjectId is required.");
    }
}
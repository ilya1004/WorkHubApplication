using FluentValidation.TestHelper;
using IdentityService.API.Contracts.UserContracts;
using IdentityService.API.Validators.UserValidators;

namespace IdentityService.Tests.UnitTests.Tests.Validators.ApiValidators.UserValidators;

public class RegisterFreelancerRequestValidatorTests
{
    private readonly RegisterFreelancerRequestValidator _validator = new();

    [Fact]
    public void Should_Pass_When_ValidRequest()
    {
        // Arrange
        var request = new RegisterFreelancerRequest("freelancer", "John", "Doe", "john@example.com", "P@ssw0rd123");

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_Fail_When_UserNameIsEmpty()
    {
        // Arrange
        var request = new RegisterFreelancerRequest("", "John", "Doe", "john@example.com", "P@ssw0rd123");

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.UserName)
            .WithErrorMessage("Username is required");
    }

    [Fact]
    public void Should_Fail_When_UserNameExceeds200Characters()
    {
        // Arrange
        var longUserName = new string('A', 201);
        var request = new RegisterFreelancerRequest(longUserName, "John", "Doe", "john@example.com", "P@ssw0rd123");

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.UserName)
            .WithErrorMessage("Username must be at most 200 characters long");
    }

    [Fact]
    public void Should_Fail_When_FirstNameIsEmpty()
    {
        // Arrange
        var request = new RegisterFreelancerRequest("freelancer", "", "Doe", "john@example.com", "P@ssw0rd123");

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.FirstName)
            .WithErrorMessage("First name is required");
    }

    [Fact]
    public void Should_Fail_When_FirstNameExceeds100Characters()
    {
        // Arrange
        var longFirstName = new string('A', 101);
        var request = new RegisterFreelancerRequest("freelancer", longFirstName, "Doe", "john@example.com", "P@ssw0rd123");

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.FirstName)
            .WithErrorMessage("First name must be at most 100 characters long");
    }

    [Fact]
    public void Should_Fail_When_LastNameIsEmpty()
    {
        // Arrange
        var request = new RegisterFreelancerRequest("freelancer", "John", "", "john@example.com", "P@ssw0rd123");

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.LastName)
            .WithErrorMessage("Last name is required");
    }

    [Fact]
    public void Should_Fail_When_LastNameExceeds100Characters()
    {
        // Arrange
        var longLastName = new string('A', 101);
        var request = new RegisterFreelancerRequest("freelancer", "John", longLastName, "john@example.com", "P@ssw0rd123");

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.LastName)
            .WithErrorMessage("Last name must be at most 100 characters long");
    }

    [Fact]
    public void Should_Fail_When_EmailIsEmpty()
    {
        // Arrange
        var request = new RegisterFreelancerRequest("freelancer", "John", "Doe", "", "P@ssw0rd123");

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("Email is required.");
    }

    [Fact]
    public void Should_Fail_When_EmailIsInvalid()
    {
        // Arrange
        var request = new RegisterFreelancerRequest("freelancer", "John", "Doe", "invalid-email", "P@ssw0rd123");

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("Invalid email format.");
    }

    [Fact]
    public void Should_Fail_When_PasswordIsEmpty()
    {
        // Arrange
        var request = new RegisterFreelancerRequest("freelancer", "John", "Doe", "john@example.com", "");

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage("Password is required.");
    }
}
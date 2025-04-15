using FluentValidation.TestHelper;
using IdentityService.API.Contracts.UserContracts;
using IdentityService.API.Validators.UserValidators;

namespace IdentityService.Tests.UnitTests.Tests.Validators.ApiValidators.UserValidators;

public class RegisterEmployerRequestValidatorTests
{
    private readonly RegisterEmployerRequestValidator _validator = new();

    [Fact]
    public void Should_Pass_When_ValidRequest()
    {
        // Arrange
        var request = new RegisterEmployerRequest("company", "Company Inc", "company@example.com", "P@ssw0rd123");

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_Fail_When_UserNameIsEmpty()
    {
        // Arrange
        var request = new RegisterEmployerRequest("", "Company Inc", "company@example.com", "P@ssw0rd123");

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.UserName)
            .WithErrorMessage("UserName is required");
    }

    [Fact]
    public void Should_Fail_When_UserNameExceeds200Characters()
    {
        // Arrange
        var longUserName = new string('A', 201);
        var request = new RegisterEmployerRequest(longUserName, "Company Inc", "company@example.com", "P@ssw0rd123");

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.UserName)
            .WithErrorMessage("UserName must not be longer than 200 characters.");
    }

    [Fact]
    public void Should_Fail_When_CompanyNameIsEmpty()
    {
        // Arrange
        var request = new RegisterEmployerRequest("company", "", "company@example.com", "P@ssw0rd123");

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.CompanyName)
            .WithErrorMessage("Company name is required");
    }

    [Fact]
    public void Should_Fail_When_CompanyNameExceeds200Characters()
    {
        // Arrange
        var longCompanyName = new string('A', 201);
        var request = new RegisterEmployerRequest("company", longCompanyName, "company@example.com", "P@ssw0rd123");

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.CompanyName)
            .WithErrorMessage("Company name must not be longer than 200 characters.");
    }

    [Fact]
    public void Should_Fail_When_EmailIsEmpty()
    {
        // Arrange
        var request = new RegisterEmployerRequest("company", "Company Inc", "", "P@ssw0rd123");

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
        var request = new RegisterEmployerRequest("company", "Company Inc", "invalid-email", "P@ssw0rd123");

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
        var request = new RegisterEmployerRequest("company", "Company Inc", "company@example.com", "");

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage("Password is required.");
    }

    [Fact]
    public void Should_Fail_When_PasswordIsTooShort()
    {
        // Arrange
        var request = new RegisterEmployerRequest("company", "Company Inc", "company@example.com", "P@ss1");

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage("Password must be at least 8 characters long.");
    }

    [Fact]
    public void Should_Fail_When_PasswordLacksLowercase()
    {
        // Arrange
        var request = new RegisterEmployerRequest("company", "Company Inc", "company@example.com", "P@SSW0RD123");

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage("Password must contain at least one lowercase letter.");
    }

    [Fact]
    public void Should_Fail_When_PasswordLacksUppercase()
    {
        // Arrange
        var request = new RegisterEmployerRequest("company", "Company Inc", "company@example.com", "p@ssw0rd123");

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage("Password must contain at least one uppercase letter.");
    }

    [Fact]
    public void Should_Fail_When_PasswordLacksDigit()
    {
        // Arrange
        var request = new RegisterEmployerRequest("company", "Company Inc", "company@example.com", "P@ssword");

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage("Password must contain at least one digit.");
    }

    [Fact]
    public void Should_Fail_When_PasswordLacksSpecialCharacter()
    {
        // Arrange
        var request = new RegisterEmployerRequest("company", "Company Inc", "company@example.com", "Passw0rd123");

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage("Password must contain at least one special character.");
    }
}
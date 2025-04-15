using FluentValidation.TestHelper;
using IdentityService.API.Contracts.AuthContracts;
using IdentityService.API.Validators.AuthValidators;

namespace IdentityService.Tests.UnitTests.Tests.Validators.ApiValidators.AuthValidators;

public class ResetPasswordRequestValidatorTests
{
    private readonly ResetPasswordRequestValidator _validator = new();

    [Fact]
    public void Should_Pass_When_ValidRequest()
    {
        // Arrange
        var request = new ResetPasswordRequest("user@example.com", "P@ssw0rd", "code123");

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_Fail_When_EmailIsEmpty()
    {
        // Arrange
        var request = new ResetPasswordRequest("", "P@ssw0rd", "code123");

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
        var request = new ResetPasswordRequest("invalid-email", "P@ssw0rd", "code123");

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("Invalid email format.");
    }

    [Fact]
    public void Should_Fail_When_NewPasswordIsEmpty()
    {
        // Arrange
        var request = new ResetPasswordRequest("user@example.com", "", "code123");

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.NewPassword)
            .WithErrorMessage("Password is required.");
    }

    [Fact]
    public void Should_Fail_When_NewPasswordIsTooShort()
    {
        // Arrange
        var request = new ResetPasswordRequest("user@example.com", "P@ss1", "code123");

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.NewPassword)
            .WithErrorMessage("Password must be at least 8 characters long.");
    }

    [Fact]
    public void Should_Fail_When_NewPasswordLacksLowercase()
    {
        // Arrange
        var request = new ResetPasswordRequest("user@example.com", "P@SSW0RD123", "code123");

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.NewPassword)
            .WithErrorMessage("Password must contain at least one lowercase letter.");
    }

    [Fact]
    public void Should_Fail_When_NewPasswordLacksUppercase()
    {
        // Arrange
        var request = new ResetPasswordRequest("user@example.com", "p@ssw0rd123", "code123");

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.NewPassword)
            .WithErrorMessage("Password must contain at least one uppercase letter.");
    }

    [Fact]
    public void Should_Fail_When_NewPasswordLacksDigit()
    {
        // Arrange
        var request = new ResetPasswordRequest("user@example.com", "P@ssword", "code123");

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.NewPassword)
            .WithErrorMessage("Password must contain at least one digit.");
    }

    [Fact]
    public void Should_Fail_When_NewPasswordLacksSpecialCharacter()
    {
        // Arrange
        var request = new ResetPasswordRequest("user@example.com", "Passw0rd123", "code123");

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.NewPassword)
            .WithErrorMessage("Password must contain at least one special character.");
    }

    [Fact]
    public void Should_Fail_When_CodeIsEmpty()
    {
        // Arrange
        var request = new ResetPasswordRequest("user@example.com", "P@ssw0rd", "");

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Code)
            .WithErrorMessage("Token is required.");
    }
}
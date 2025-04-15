using FluentValidation.TestHelper;
using IdentityService.API.Contracts.UserContracts;
using IdentityService.API.Validators.UserValidators;

namespace IdentityService.Tests.UnitTests.Tests.Validators.ApiValidators.UserValidators;

public class ChangePasswordRequestValidatorTests
{
    private readonly ChangePasswordRequestValidator _validator = new();

    [Fact]
    public void Should_Pass_When_ValidRequest()
    {
        // Arrange
        var request = new ChangePasswordRequest("user@example.com", "Current@123", "NewP@ssw0rd");

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_Fail_When_EmailIsEmpty()
    {
        // Arrange
        var request = new ChangePasswordRequest("", "Current@123", "NewP@ssw0rd");

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
        var request = new ChangePasswordRequest("invalid-email", "Current@123", "NewP@ssw0rd");

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("Invalid email format.");
    }

    [Fact]
    public void Should_Fail_When_CurrentPasswordIsEmpty()
    {
        // Arrange
        var request = new ChangePasswordRequest("user@example.com", "", "NewP@ssw0rd");

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.CurrentPassword)
            .WithErrorMessage("Current password is required.");
    }

    [Fact]
    public void Should_Fail_When_NewPasswordIsEmpty()
    {
        // Arrange
        var request = new ChangePasswordRequest("user@example.com", "Current@123", "");

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.NewPassword)
            .WithErrorMessage("New password is required.");
    }

    [Fact]
    public void Should_Fail_When_NewPasswordIsTooShort()
    {
        // Arrange
        var request = new ChangePasswordRequest("user@example.com", "Current@123", "P@ss1");

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.NewPassword)
            .WithErrorMessage("New password must be at least 8 characters long.");
    }

    [Fact]
    public void Should_Fail_When_NewPasswordLacksLowercase()
    {
        // Arrange
        var request = new ChangePasswordRequest("user@example.com", "Current@123", "P@SSW0RD123");

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.NewPassword)
            .WithErrorMessage("New password must contain at least one lowercase letter.");
    }

    [Fact]
    public void Should_Fail_When_NewPasswordLacksUppercase()
    {
        // Arrange
        var request = new ChangePasswordRequest("user@example.com", "Current@123", "p@ssw0rd123");

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.NewPassword)
            .WithErrorMessage("New password must contain at least one uppercase letter.");
    }

    [Fact]
    public void Should_Fail_When_NewPasswordLacksDigit()
    {
        // Arrange
        var request = new ChangePasswordRequest("user@example.com", "Current@123", "P@ssword");

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.NewPassword)
            .WithErrorMessage("New password must contain at least one digit.");
    }

    [Fact]
    public void Should_Fail_When_NewPasswordLacksSpecialCharacter()
    {
        // Arrange
        var request = new ChangePasswordRequest("user@example.com", "Current@123", "Passw0rd123");

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.NewPassword)
            .WithErrorMessage("New password must contain at least one special character.");
    }
}
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
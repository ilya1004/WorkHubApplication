using FluentValidation.TestHelper;
using IdentityService.API.Contracts.AuthContracts;
using IdentityService.API.Validators.AuthValidators;

namespace IdentityService.Tests.UnitTests.Tests.Validators.ApiValidators.AuthValidators;

public class ForgotPasswordRequestValidatorTests
{
    private readonly ForgotPasswordRequestValidator _validator = new();

    [Fact]
    public void Should_Pass_When_ValidRequest()
    {
        // Arrange
        var request = new ForgotPasswordRequest("user@example.com", "https://reset.url");

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_Fail_When_EmailIsEmpty()
    {
        // Arrange
        var request = new ForgotPasswordRequest("", "https://reset.url");

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
        var request = new ForgotPasswordRequest("invalid-email", "https://reset.url");

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("Invalid email format.");
    }

    [Fact]
    public void Should_Fail_When_ResetUrlIsEmpty()
    {
        // Arrange
        var request = new ForgotPasswordRequest("user@example.com", "");

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ResetUrl)
            .WithErrorMessage("Reset Url is required.");
    }

    [Fact]
    public void Should_Fail_When_ResetUrlIsInvalid()
    {
        // Arrange
        var request = new ForgotPasswordRequest("user@example.com", "invalid-url");

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ResetUrl)
            .WithErrorMessage("Invalid url format.");
    }
}
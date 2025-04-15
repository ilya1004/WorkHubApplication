using FluentValidation.TestHelper;
using IdentityService.API.Contracts.AuthContracts;
using IdentityService.API.Validators.AuthValidators;

namespace IdentityService.Tests.UnitTests.Tests.Validators.ApiValidators.AuthValidators;

public class LoginUserRequestValidatorTests
{
    private readonly LoginUserRequestValidator _validator = new();

    [Fact]
    public void Should_Pass_When_ValidRequest()
    {
        // Arrange
        var request = new LoginUserRequest("user@example.com", "password123");

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_Fail_When_EmailIsEmpty()
    {
        // Arrange
        var request = new LoginUserRequest("", "password123");

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
        var request = new LoginUserRequest("invalid-email", "password123");

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
        var request = new LoginUserRequest("user@example.com", "");

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage("Password is required.");
    }
}
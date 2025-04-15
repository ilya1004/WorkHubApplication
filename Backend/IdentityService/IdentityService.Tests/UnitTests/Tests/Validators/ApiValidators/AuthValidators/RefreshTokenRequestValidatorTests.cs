using FluentValidation.TestHelper;
using IdentityService.API.Contracts.AuthContracts;
using IdentityService.API.Validators.AuthValidators;

namespace IdentityService.Tests.UnitTests.Tests.Validators.ApiValidators.AuthValidators;

public class RefreshTokenRequestValidatorTests
{
    private readonly RefreshTokenRequestValidator _validator = new();

    [Fact]
    public void Should_Pass_When_ValidRequest()
    {
        // Arrange
        var request = new RefreshTokenRequest("header.payload.signature", "refresh_token");

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_Fail_When_AccessTokenIsEmpty()
    {
        // Arrange
        var request = new RefreshTokenRequest("", "refresh_token");

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.AccessToken)
            .WithErrorMessage("Access token is required.");
    }

    [Fact]
    public void Should_Fail_When_AccessTokenIsInvalid()
    {
        // Arrange
        var request = new RefreshTokenRequest("invalid_token", "refresh_token");

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.AccessToken)
            .WithErrorMessage("Invalid access token format.");
    }

    [Fact]
    public void Should_Fail_When_RefreshTokenIsEmpty()
    {
        // Arrange
        var request = new RefreshTokenRequest("header.payload.signature", "");

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.RefreshToken)
            .WithErrorMessage("Refresh token is required.");
    }
}
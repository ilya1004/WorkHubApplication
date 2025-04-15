using FluentValidation.TestHelper;
using IdentityService.API.Contracts.UserContracts;
using IdentityService.API.Validators.UserValidators;

namespace IdentityService.Tests.UnitTests.Tests.Validators.ApiValidators.UserValidators;

public class UpdateEmployerProfileRequestValidatorTests
{
    private readonly UpdateEmployerProfileRequestValidator _validator = new();

    [Fact]
    public void Should_Pass_When_ValidRequest()
    {
        // Arrange
        var profileDto = new EmployerProfileDto("Company Inc", "About company", Guid.NewGuid(), false);
        var request = new UpdateEmployerProfileRequest(profileDto, null);

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_Fail_When_CompanyNameIsEmpty()
    {
        // Arrange
        var profileDto = new EmployerProfileDto("", "About company", Guid.NewGuid(), false);
        var request = new UpdateEmployerProfileRequest(profileDto, null);

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.EmployerProfile.CompanyName)
            .WithErrorMessage("Company name is required");
    }

    [Fact]
    public void Should_Fail_When_CompanyNameExceeds200Characters()
    {
        // Arrange
        var longCompanyName = new string('A', 201);
        var profileDto = new EmployerProfileDto(longCompanyName, "About company", Guid.NewGuid(), false);
        var request = new UpdateEmployerProfileRequest(profileDto, null);

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.EmployerProfile.CompanyName)
            .WithErrorMessage("Company name must not be longer than 200 characters.");
    }

    [Fact]
    public void Should_Fail_When_AboutExceeds1000Characters()
    {
        // Arrange
        var longAbout = new string('A', 1001);
        var profileDto = new EmployerProfileDto("Company Inc", longAbout, Guid.NewGuid(), false);
        var request = new UpdateEmployerProfileRequest(profileDto, null);

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.EmployerProfile.About)
            .WithErrorMessage("'About' value must not be longer than 1000 characters.");
    }

    [Fact]
    public void Should_Pass_When_AboutIsNull()
    {
        // Arrange
        var profileDto = new EmployerProfileDto("Company Inc", null, Guid.NewGuid(), false);
        var request = new UpdateEmployerProfileRequest(profileDto, null);

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
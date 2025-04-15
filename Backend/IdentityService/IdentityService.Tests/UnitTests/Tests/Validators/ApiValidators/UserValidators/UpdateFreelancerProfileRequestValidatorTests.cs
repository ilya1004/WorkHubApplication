using FluentValidation.TestHelper;
using IdentityService.API.Contracts.UserContracts;
using IdentityService.API.Validators.UserValidators;

namespace IdentityService.Tests.UnitTests.Tests.Validators.ApiValidators.UserValidators;

public class UpdateFreelancerProfileRequestValidatorTests
{
    private readonly UpdateFreelancerProfileRequestValidator _validator = new();

    [Fact]
    public void Should_Pass_When_ValidRequest()
    {
        // Arrange
        var profileDto = new FreelancerProfileDto("John", "Doe", "About me", [Guid.NewGuid()], false);
        var request = new UpdateFreelancerProfileRequest(profileDto, null);

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_Fail_When_FirstNameIsEmpty()
    {
        // Arrange
        var profileDto = new FreelancerProfileDto("", "Doe", "About me", [Guid.NewGuid()], false);
        var request = new UpdateFreelancerProfileRequest(profileDto, null);

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.FreelancerProfile.FirstName)
            .WithErrorMessage("First name is required");
    }

    [Fact]
    public void Should_Fail_When_FirstNameExceeds100Characters()
    {
        // Arrange
        var longFirstName = new string('A', 101);
        var profileDto = new FreelancerProfileDto(longFirstName, "Doe", "About me", [Guid.NewGuid()], false);
        var request = new UpdateFreelancerProfileRequest(profileDto, null);

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.FreelancerProfile.FirstName)
            .WithErrorMessage("First name must be at most 100 characters long");
    }

    [Fact]
    public void Should_Fail_When_LastNameIsEmpty()
    {
        // Arrange
        var profileDto = new FreelancerProfileDto("John", "", "About me", [Guid.NewGuid()], false);
        var request = new UpdateFreelancerProfileRequest(profileDto, null);

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.FreelancerProfile.LastName)
            .WithErrorMessage("Last name is required");
    }

    [Fact]
    public void Should_Fail_When_LastNameExceeds100Characters()
    {
        // Arrange
        var longLastName = new string('A', 101);
        var profileDto = new FreelancerProfileDto("John", longLastName, "About me", [Guid.NewGuid()], false);
        var request = new UpdateFreelancerProfileRequest(profileDto, null);

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.FreelancerProfile.LastName)
            .WithErrorMessage("Last name must be at most 100 characters long");
    }

    [Fact]
    public void Should_Fail_When_AboutExceeds1000Characters()
    {
        // Arrange
        var longAbout = new string('A', 1001);
        var profileDto = new FreelancerProfileDto("John", "Doe", longAbout, [Guid.NewGuid()], false);
        var request = new UpdateFreelancerProfileRequest(profileDto, null);

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.FreelancerProfile.About)
            .WithErrorMessage("'About' value must not be longer than 1000 characters.");
    }

    [Fact]
    public void Should_Pass_When_AboutIsNull()
    {
        // Arrange
        var profileDto = new FreelancerProfileDto("John", "Doe", null, [Guid.NewGuid()], false);
        var request = new UpdateFreelancerProfileRequest(profileDto, null);

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
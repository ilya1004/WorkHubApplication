using FluentValidation.TestHelper;
using IdentityService.API.DTOs;
using IdentityService.API.Validators.EmployerIndustryValidators;

namespace IdentityService.Tests.UnitTests.Tests.Validators.ApiValidators.EmployerIndustryValidators;

public class EmployerIndustryDtoValidatorTests
{
    private readonly EmployerIndustryDtoValidator _validator = new();

    [Fact]
    public void Should_Pass_When_ValidDto()
    {
        // Arrange
        var dto = new EmployerIndustryDataDto("Technology");

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_Fail_When_NameIsEmpty()
    {
        // Arrange
        var dto = new EmployerIndustryDataDto("");

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("Name is required");
    }

    [Fact]
    public void Should_Fail_When_NameIsNull()
    {
        // Arrange
        var dto = new EmployerIndustryDataDto(null!);

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("Name is required");
    }

    [Fact]
    public void Should_Fail_When_NameExceeds200Characters()
    {
        // Arrange
        var longName = new string('A', 201);
        var dto = new EmployerIndustryDataDto(longName);

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("Name must not be longer than 200 characters.");
    }
}
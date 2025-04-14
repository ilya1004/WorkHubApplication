using FluentValidation.TestHelper;
using ProjectsService.API.Validators.CategoryValidators;

namespace ProjectsService.Tests.UnitTests.Tests.Validators.CategoryValidators;

public class CategoryDtoValidatorTests
{
    private readonly CategoryDtoValidator _validator = new();

    [Fact]
    public void Validate_ValidName_Succeeds()
    {
        // Arrange
        var dto = new CategoryDto("Test Category");

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_EmptyName_FailsWithCorrectMessage()
    {
        // Arrange
        var dto = new CategoryDto("");

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("Name cannot be empty");
    }

    [Fact]
    public void Validate_NullName_FailsWithCorrectMessage()
    {
        // Arrange
        var dto = new CategoryDto(null!);

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("Name cannot be empty");
    }

    [Fact]
    public void Validate_NameExceeds200Characters_FailsWithCorrectMessage()
    {
        // Arrange
        var longName = new string('A', 201);
        var dto = new CategoryDto(longName);

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("Name must not exceed 200 characters.");
    }

    [Fact]
    public void Validate_NameExactly200Characters_Succeeds()
    {
        // Arrange
        var name = new string('A', 200);
        var dto = new CategoryDto(name);

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
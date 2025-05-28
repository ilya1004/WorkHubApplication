using PaymentsService.Application.UseCases.PaymentsUseCases.Commands.PayForProjectWithSavedMethod;
using PaymentsService.Application.Validators.PaymentsValidators;

namespace PaymentsService.Tests.UnitTests.Tests.Validators.PaymentsValidators;

public class PayForProjectWithSavedMethodCommandValidatorTests
{
    private readonly PayForProjectWithSavedMethodCommandValidator _validator = new();

    [Fact]
    public void Validate_ValidCommand_PassesValidation()
    {
        // Arrange
        var command = new PayForProjectWithSavedMethodCommand(
            ProjectId: Guid.NewGuid(),
            PaymentMethodId: "pm_1Q0PsIJvEtkwdCNYMSaVuRz6");

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void Validate_EmptyProjectId_FailsWithRequiredError()
    {
        // Arrange
        var command = new PayForProjectWithSavedMethodCommand(
            ProjectId: Guid.Empty,
            PaymentMethodId: "pm_1Q0PsIJvEtkwdCNYMSaVuRz6");

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCount(1);
        result.Errors[0].PropertyName.Should().Be("ProjectId");
        result.Errors[0].ErrorMessage.Should().Be("ProjectId is required");
    }

    [Fact]
    public void Validate_NullPaymentMethodId_FailsWithRequiredError()
    {
        // Arrange
        var command = new PayForProjectWithSavedMethodCommand(
            ProjectId: Guid.NewGuid(),
            PaymentMethodId: null!);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCount(1);
        result.Errors[0].PropertyName.Should().Be("PaymentMethodId");
        result.Errors[0].ErrorMessage.Should().Be("PaymentMethodId is required");
    }

    [Fact]
    public void Validate_WrongPrefixPaymentMethodId_FailsWithFormatError()
    {
        // Arrange
        var command = new PayForProjectWithSavedMethodCommand(
            ProjectId: Guid.NewGuid(),
            PaymentMethodId: "xxxx_1Q0PsIJvEtkwdCNYMSaVuRz6");

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCount(1);
        result.Errors[0].PropertyName.Should().Be("PaymentMethodId");
        result.Errors[0].ErrorMessage.Should().Be("Invalid PaymentMethodId format");
    }
}
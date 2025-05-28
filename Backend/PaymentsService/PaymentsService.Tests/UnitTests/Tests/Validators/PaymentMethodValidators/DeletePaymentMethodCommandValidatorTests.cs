using PaymentsService.Application.UseCases.PaymentMethodUseCases.Commands.DeletePaymentMethod;
using PaymentsService.Application.Validators.PaymentMethodValidators;

namespace PaymentsService.Tests.UnitTests.Tests.Validators.PaymentMethodValidators;

public class DeletePaymentMethodCommandValidatorTests
{
    private readonly DeletePaymentMethodCommandValidator _validator = new();

    [Fact]
    public void Validate_ValidPaymentMethodId_PassesValidation()
    {
        // Arrange
        var command = new DeletePaymentMethodCommand("pm_1Q0PsIJvEtkwdCNYMSaVuRz6");

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void Validate_NullPaymentMethodId_FailsWithRequiredError()
    {
        // Arrange
        var command = new DeletePaymentMethodCommand(null!);

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
        var command = new DeletePaymentMethodCommand("xxxx_1Q0PsIJvEtkwdCNYMSaVuRz6");

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCount(1);
        result.Errors[0].PropertyName.Should().Be("PaymentMethodId");
        result.Errors[0].ErrorMessage.Should().Be("Invalid PaymentMethodId format");
    }
}
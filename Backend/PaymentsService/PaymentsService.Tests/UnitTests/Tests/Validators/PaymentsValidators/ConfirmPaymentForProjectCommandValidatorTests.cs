using PaymentsService.Application.UseCases.PaymentsUseCases.Commands.ConfirmPaymentForProject;
using PaymentsService.Application.Validators.PaymentsValidators;

namespace PaymentsService.Tests.UnitTests.Tests.Validators.PaymentsValidators;

public class ConfirmPaymentForProjectCommandValidatorTests
{
    private readonly ConfirmPaymentForProjectCommandValidator _validator = new();

    [Fact]
    public void Validate_ValidProjectId_PassesValidation()
    {
        // Arrange
        var command = new ConfirmPaymentForProjectCommand(Guid.NewGuid());

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
        var command = new ConfirmPaymentForProjectCommand(Guid.Empty);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCount(1);
        result.Errors[0].PropertyName.Should().Be("ProjectId");
        result.Errors[0].ErrorMessage.Should().Be("ProjectId is required");
    }
}
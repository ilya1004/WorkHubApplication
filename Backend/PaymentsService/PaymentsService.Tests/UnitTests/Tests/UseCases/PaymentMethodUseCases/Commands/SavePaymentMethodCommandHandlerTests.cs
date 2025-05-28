using PaymentsService.Application.UseCases.PaymentMethodUseCases.Commands.SavePaymentMethod;
using PaymentsService.Domain.Abstractions.PaymentsServices;
using PaymentsService.Domain.Abstractions.UserContext;
using PaymentsService.Tests.UnitTests.Extensions;

namespace PaymentsService.Tests.UnitTests.Tests.UseCases.PaymentMethodUseCases.Commands;

public class SavePaymentMethodCommandHandlerTests
{
    private readonly Mock<IPaymentMethodsService> _paymentMethodsServiceMock;
    private readonly Mock<IUserContext> _userContextMock;
    private readonly Mock<ILogger<SavePaymentMethodCommandHandler>> _loggerMock;
    private readonly SavePaymentMethodCommandHandler _handler;

    public SavePaymentMethodCommandHandlerTests()
    {
        _paymentMethodsServiceMock = new Mock<IPaymentMethodsService>();
        _userContextMock = new Mock<IUserContext>();
        _loggerMock = new Mock<ILogger<SavePaymentMethodCommandHandler>>();
        _handler = new SavePaymentMethodCommandHandler(
            _paymentMethodsServiceMock.Object,
            _userContextMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_SavesPaymentMethod_Successfully()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var paymentMethodId = "pm_456";
        var command = new SavePaymentMethodCommand(paymentMethodId);
        _userContextMock.Setup(uc => uc.GetUserId()).Returns(userId);
        _paymentMethodsServiceMock.Setup(s => s.SavePaymentMethodAsync(userId, paymentMethodId, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _paymentMethodsServiceMock.Verify(s => s.SavePaymentMethodAsync(userId, paymentMethodId, It.IsAny<CancellationToken>()), Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Starting to save payment method {paymentMethodId} for user {userId}", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Successfully saved payment method {paymentMethodId} for user {userId}", Times.Once());
    }
}
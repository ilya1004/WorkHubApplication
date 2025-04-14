using PaymentsService.Application.UseCases.PaymentMethodUseCases.Commands.DeletePaymentMethod;
using PaymentsService.Domain.Abstractions.PaymentsServices;
using PaymentsService.Domain.Abstractions.UserContext;
using PaymentsService.Tests.UnitTests.Extensions;

namespace PaymentsService.Tests.UnitTests.Tests.UseCases.PaymentMethodUseCases.Commands;

public class DeletePaymentMethodCommandHandlerTests
{
    private readonly Mock<IPaymentMethodsService> _paymentMethodsServiceMock;
    private readonly Mock<IUserContext> _userContextMock;
    private readonly Mock<ILogger<DeletePaymentMethodCommandHandler>> _loggerMock;
    private readonly DeletePaymentMethodCommandHandler _handler;

    public DeletePaymentMethodCommandHandlerTests()
    {
        _paymentMethodsServiceMock = new Mock<IPaymentMethodsService>();
        _userContextMock = new Mock<IUserContext>();
        _loggerMock = new Mock<ILogger<DeletePaymentMethodCommandHandler>>();
        _handler = new DeletePaymentMethodCommandHandler(
            _paymentMethodsServiceMock.Object,
            _userContextMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_DeletesPaymentMethod_Successfully()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var paymentMethodId = "pm_123";
        var command = new DeletePaymentMethodCommand(paymentMethodId);
        _userContextMock.Setup(uc => uc.GetUserId()).Returns(userId);
        _paymentMethodsServiceMock.Setup(s => s.DeletePaymentMethodAsync(userId, paymentMethodId, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _paymentMethodsServiceMock.Verify(s => s.DeletePaymentMethodAsync(userId, paymentMethodId, It.IsAny<CancellationToken>()), Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Starting to delete payment method {paymentMethodId} for user {userId}", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Successfully deleted payment method {paymentMethodId} for user {userId}", Times.Once());
    }
}
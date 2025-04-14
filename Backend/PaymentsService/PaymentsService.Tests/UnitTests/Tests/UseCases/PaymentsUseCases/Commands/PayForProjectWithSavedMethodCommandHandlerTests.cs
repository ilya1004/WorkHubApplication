using PaymentsService.Application.UseCases.PaymentsUseCases.Commands.PayForProjectWithSavedMethod;
using PaymentsService.Domain.Abstractions.PaymentsServices;
using PaymentsService.Domain.Abstractions.UserContext;
using PaymentsService.Tests.UnitTests.Extensions;

namespace PaymentsService.Tests.UnitTests.Tests.UseCases.PaymentsUseCases.Commands;

public class PayForProjectWithSavedMethodCommandHandlerTests
{
    private readonly Mock<IEmployerPaymentsService> _employerPaymentsServiceMock;
    private readonly Mock<IUserContext> _userContextMock;
    private readonly Mock<ILogger<PayForProjectWithSavedMethodCommandHandler>> _loggerMock;
    private readonly PayForProjectWithSavedMethodCommandHandler _handler;

    public PayForProjectWithSavedMethodCommandHandlerTests()
    {
        _employerPaymentsServiceMock = new Mock<IEmployerPaymentsService>();
        _userContextMock = new Mock<IUserContext>();
        _loggerMock = new Mock<ILogger<PayForProjectWithSavedMethodCommandHandler>>();
        _handler = new PayForProjectWithSavedMethodCommandHandler(
            _employerPaymentsServiceMock.Object,
            _userContextMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_CreatesPaymentIntent_Successfully()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var paymentMethodId = "pm_123";
        var command = new PayForProjectWithSavedMethodCommand(projectId, paymentMethodId);
        _userContextMock.Setup(uc => uc.GetUserId()).Returns(userId);
        _employerPaymentsServiceMock.Setup(s => s.CreatePaymentIntentWithSavedMethodAsync(userId, projectId, paymentMethodId, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _employerPaymentsServiceMock.Verify(s => s.CreatePaymentIntentWithSavedMethodAsync(userId, projectId, paymentMethodId, It.IsAny<CancellationToken>()), Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Processing payment for project {projectId} with saved method {paymentMethodId} by user {userId}", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Payment for project {projectId} with saved method {paymentMethodId} processed successfully by user {userId}", Times.Once());
    }
}
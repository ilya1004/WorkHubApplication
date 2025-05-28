using PaymentsService.Application.UseCases.PaymentsUseCases.Commands.ConfirmPaymentForProject;
using PaymentsService.Domain.Abstractions.PaymentsServices;
using PaymentsService.Domain.Abstractions.UserContext;
using PaymentsService.Tests.UnitTests.Extensions;

namespace PaymentsService.Tests.UnitTests.Tests.UseCases.PaymentsUseCases.Commands;

public class ConfirmPaymentForProjectCommandHandlerTests
{
    private readonly Mock<IEmployerPaymentsService> _employerPaymentsServiceMock;
    private readonly Mock<IUserContext> _userContextMock;
    private readonly Mock<ILogger<ConfirmPaymentForProjectCommandHandler>> _loggerMock;
    private readonly ConfirmPaymentForProjectCommandHandler _handler;

    public ConfirmPaymentForProjectCommandHandlerTests()
    {
        _employerPaymentsServiceMock = new Mock<IEmployerPaymentsService>();
        _userContextMock = new Mock<IUserContext>();
        _loggerMock = new Mock<ILogger<ConfirmPaymentForProjectCommandHandler>>();
        _handler = new ConfirmPaymentForProjectCommandHandler(
            _employerPaymentsServiceMock.Object,
            _userContextMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ConfirmsPayment_Successfully()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var command = new ConfirmPaymentForProjectCommand(projectId);
        _userContextMock.Setup(uc => uc.GetUserId()).Returns(userId);
        _employerPaymentsServiceMock.Setup(s => s.ConfirmPaymentForProjectAsync(projectId, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _employerPaymentsServiceMock.Verify(s => s.ConfirmPaymentForProjectAsync(projectId, It.IsAny<CancellationToken>()), Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Confirming payment for project {projectId} by user {userId}", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Payment for project {projectId} confirmed successfully by user {userId}", Times.Once());
    }
}
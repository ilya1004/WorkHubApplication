using PaymentsService.Application.UseCases.AccountUseCases.Commands.CreateEmployerAccount;
using PaymentsService.Domain.Abstractions.AccountsServices;
using PaymentsService.Domain.Abstractions.KafkaProducerServices;
using PaymentsService.Domain.Abstractions.UserContext;
using PaymentsService.Tests.UnitTests.Extensions;

namespace PaymentsService.Tests.UnitTests.Tests.UseCases.AccountUseCases.Commands;

public class CreateEmployerAccountCommandHandlerTests
{
    private readonly Mock<IEmployerAccountsService> _employerAccountsServiceMock;
    private readonly Mock<IUserContext> _userContextMock;
    private readonly Mock<IAccountsProducerService> _accountsProducerServiceMock;
    private readonly Mock<ILogger<CreateEmployerAccountCommandHandler>> _loggerMock;
    private readonly CreateEmployerAccountCommandHandler _handler;

    public CreateEmployerAccountCommandHandlerTests()
    {
        _employerAccountsServiceMock = new Mock<IEmployerAccountsService>();
        _userContextMock = new Mock<IUserContext>();
        _accountsProducerServiceMock = new Mock<IAccountsProducerService>();
        _loggerMock = new Mock<ILogger<CreateEmployerAccountCommandHandler>>();
        _handler = new CreateEmployerAccountCommandHandler(
            _employerAccountsServiceMock.Object,
            _userContextMock.Object,
            _accountsProducerServiceMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_CreatesEmployerAccount_Successfully()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var userEmail = "employer@example.com";
        var accountId = "acc_123";
        var command = new CreateEmployerAccountCommand();

        _userContextMock.Setup(uc => uc.GetUserId()).Returns(userId);
        _userContextMock.Setup(uc => uc.GetUserEmail()).Returns(userEmail);
        _employerAccountsServiceMock.Setup(s => s.CreateEmployerAccountAsync(userId, userEmail, It.IsAny<CancellationToken>()))
            .ReturnsAsync(accountId);
        _accountsProducerServiceMock.Setup(p => p.SaveEmployerAccountIdAsync(userId.ToString(), accountId, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _employerAccountsServiceMock.Verify(s => s.CreateEmployerAccountAsync(userId, userEmail, It.IsAny<CancellationToken>()), Times.Once());
        _accountsProducerServiceMock.Verify(p => p.SaveEmployerAccountIdAsync(userId.ToString(), accountId, It.IsAny<CancellationToken>()), Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Creating employer account for user {userId} with email {userEmail}", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Successfully created employer account with ID {accountId} for user {userId}", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Saving employer account ID {accountId} to producer service for user {userId}", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Successfully processed employer account creation for user {userId}", Times.Once());
    }
}
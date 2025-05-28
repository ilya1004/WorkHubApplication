using PaymentsService.Application.UseCases.AccountUseCases.Commands.CreateFreelancerAccount;
using PaymentsService.Domain.Abstractions.AccountsServices;
using PaymentsService.Domain.Abstractions.KafkaProducerServices;
using PaymentsService.Domain.Abstractions.UserContext;
using PaymentsService.Tests.UnitTests.Extensions;

namespace PaymentsService.Tests.UnitTests.Tests.UseCases.AccountUseCases.Commands;

public class CreateFreelancerAccountCommandHandlerTests
{
    private readonly Mock<IFreelancerAccountsService> _freelancerAccountsServiceMock;
    private readonly Mock<IUserContext> _userContextMock;
    private readonly Mock<IAccountsProducerService> _accountsProducerServiceMock;
    private readonly Mock<ILogger<CreateFreelancerAccountCommandHandler>> _loggerMock;
    private readonly CreateFreelancerAccountCommandHandler _handler;

    public CreateFreelancerAccountCommandHandlerTests()
    {
        _freelancerAccountsServiceMock = new Mock<IFreelancerAccountsService>();
        _userContextMock = new Mock<IUserContext>();
        _accountsProducerServiceMock = new Mock<IAccountsProducerService>();
        _loggerMock = new Mock<ILogger<CreateFreelancerAccountCommandHandler>>();
        _handler = new CreateFreelancerAccountCommandHandler(
            _freelancerAccountsServiceMock.Object,
            _userContextMock.Object,
            _accountsProducerServiceMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_CreatesFreelancerAccount_Successfully()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var userEmail = "freelancer@example.com";
        var accountId = "acc_456";
        var command = new CreateFreelancerAccountCommand();

        _userContextMock.Setup(uc => uc.GetUserId()).Returns(userId);
        _userContextMock.Setup(uc => uc.GetUserEmail()).Returns(userEmail);
        _freelancerAccountsServiceMock.Setup(s => s.CreateFreelancerAccountAsync(userId, userEmail, It.IsAny<CancellationToken>()))
            .ReturnsAsync(accountId);
        _accountsProducerServiceMock.Setup(p => p.SaveFreelancerAccountIdAsync(userId.ToString(), accountId, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _freelancerAccountsServiceMock.Verify(s => s.CreateFreelancerAccountAsync(userId, userEmail, It.IsAny<CancellationToken>()), Times.Once());
        _accountsProducerServiceMock.Verify(p => p.SaveFreelancerAccountIdAsync(userId.ToString(), accountId, It.IsAny<CancellationToken>()), Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Creating freelancer account for user {userId} with email {userEmail}", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Successfully created freelancer account with ID {accountId} for user {userId}", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Saving freelancer account ID {accountId} to producer service for user {userId}", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Successfully processed freelancer account creation for user {userId}", Times.Once());
    }
}
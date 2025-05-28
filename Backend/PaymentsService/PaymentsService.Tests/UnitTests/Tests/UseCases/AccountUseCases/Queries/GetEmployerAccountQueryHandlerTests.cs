using PaymentsService.Application.UseCases.AccountUseCases.Queries.GetEmployerAccount;
using PaymentsService.Domain.Abstractions.AccountsServices;
using PaymentsService.Domain.Abstractions.UserContext;
using PaymentsService.Domain.Models;
using PaymentsService.Tests.UnitTests.Extensions;

namespace PaymentsService.Tests.UnitTests.Tests.UseCases.AccountUseCases.Queries;

public class GetEmployerAccountQueryHandlerTests
{
    private readonly Mock<IEmployerAccountsService> _employerAccountsServiceMock;
    private readonly Mock<IUserContext> _userContextMock;
    private readonly Mock<ILogger<GetEmployerAccountQueryHandler>> _loggerMock;
    private readonly GetEmployerAccountQueryHandler _handler;

    public GetEmployerAccountQueryHandlerTests()
    {
        _employerAccountsServiceMock = new Mock<IEmployerAccountsService>();
        _userContextMock = new Mock<IUserContext>();
        _loggerMock = new Mock<ILogger<GetEmployerAccountQueryHandler>>();
        _handler = new GetEmployerAccountQueryHandler(_employerAccountsServiceMock.Object, _userContextMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ReturnsEmployerAccount()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var account = new EmployerAccountModel
        {
            Id = "acc_123",
            OwnerEmail = "employer@example.com",
            Currency = "USD",
            Balance = 1000
        };
        var query = new GetEmployerAccountQuery();
        _userContextMock.Setup(uc => uc.GetUserId()).Returns(userId);
        _employerAccountsServiceMock.Setup(s => s.GetEmployerAccountAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(account);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(account);
        _employerAccountsServiceMock.Verify(s => s.GetEmployerAccountAsync(userId, It.IsAny<CancellationToken>()), Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Retrieving employer account for user {userId}", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Successfully retrieved employer account for user {userId}", Times.Once());
    }
}
using PaymentsService.Application.UseCases.AccountUseCases.Queries.GetFreelancerAccount;
using PaymentsService.Domain.Abstractions.AccountsServices;
using PaymentsService.Domain.Abstractions.UserContext;
using PaymentsService.Domain.Models;
using PaymentsService.Tests.UnitTests.Extensions;

namespace PaymentsService.Tests.UnitTests.Tests.UseCases.AccountUseCases.Queries;

public class GetFreelancerAccountQueryHandlerTests
{
    private readonly Mock<IFreelancerAccountsService> _freelancerAccountsServiceMock;
    private readonly Mock<IUserContext> _userContextMock;
    private readonly Mock<ILogger<GetFreelancerAccountQueryHandler>> _loggerMock;
    private readonly GetFreelancerAccountQueryHandler _handler;

    public GetFreelancerAccountQueryHandlerTests()
    {
        _freelancerAccountsServiceMock = new Mock<IFreelancerAccountsService>();
        _userContextMock = new Mock<IUserContext>();
        _loggerMock = new Mock<ILogger<GetFreelancerAccountQueryHandler>>();
        _handler = new GetFreelancerAccountQueryHandler(_freelancerAccountsServiceMock.Object, _userContextMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ReturnsFreelancerAccount()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var account = new FreelancerAccountModel
        {
            Id = "acc_456",
            OwnerEmail = "freelancer@example.com",
            AccountType = "individual",
            Country = "US",
            Balance = 500
        };
        var query = new GetFreelancerAccountQuery();
        _userContextMock.Setup(uc => uc.GetUserId()).Returns(userId);
        _freelancerAccountsServiceMock.Setup(s => s.GetFreelancerAccountAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(account);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(account);
        _freelancerAccountsServiceMock.Verify(s => s.GetFreelancerAccountAsync(userId, It.IsAny<CancellationToken>()), Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Retrieving freelancer account for user {userId}", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Successfully retrieved freelancer account for user {userId}", Times.Once());
    }
}
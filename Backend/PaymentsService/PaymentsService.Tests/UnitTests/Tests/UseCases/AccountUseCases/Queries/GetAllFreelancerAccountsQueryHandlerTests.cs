using PaymentsService.Application.UseCases.AccountUseCases.Queries.GetAllFreelancerAccounts;
using PaymentsService.Domain.Abstractions.AccountsServices;
using PaymentsService.Domain.Models;
using PaymentsService.Tests.UnitTests.Extensions;

namespace PaymentsService.Tests.UnitTests.Tests.UseCases.AccountUseCases.Queries;

public class GetAllFreelancerAccountsQueryHandlerTests
{
    private readonly Mock<IFreelancerAccountsService> _freelancerAccountsServiceMock;
    private readonly Mock<ILogger<GetAllFreelancerAccountsQueryHandler>> _loggerMock;
    private readonly GetAllFreelancerAccountsQueryHandler _handler;

    public GetAllFreelancerAccountsQueryHandlerTests()
    {
        _freelancerAccountsServiceMock = new Mock<IFreelancerAccountsService>();
        _loggerMock = new Mock<ILogger<GetAllFreelancerAccountsQueryHandler>>();
        _handler = new GetAllFreelancerAccountsQueryHandler(_freelancerAccountsServiceMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ReturnsPaginatedFreelancerAccounts()
    {
        // Arrange
        var query = new GetAllFreelancerAccountsQuery(PageNo: 2, PageSize: 3);
        var accounts = new List<FreelancerAccountModel>
        {
            new() { Id = "acc_1", OwnerEmail = "freelancer1@example.com", AccountType = "individual", Country = "US", Balance = 1000 },
            new() { Id = "acc_2", OwnerEmail = "freelancer2@example.com", AccountType = "business", Country = "UK", Balance = 2000 },
            new() { Id = "acc_3", OwnerEmail = "freelancer3@example.com", AccountType = "individual", Country = "CA", Balance = 3000 },
            new() { Id = "acc_4", OwnerEmail = "freelancer4@example.com", AccountType = "business", Country = "AU", Balance = 4000 }
        };
        _freelancerAccountsServiceMock.Setup(s => s.GetAllFreelancerAccountsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(accounts);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(1);
        result.Items[0].Should().BeEquivalentTo(accounts[3]);
        result.PageNo.Should().Be(query.PageNo);
        result.PageSize.Should().Be(query.PageSize);
        result.TotalCount.Should().Be(1);
        _freelancerAccountsServiceMock.Verify(s => s.GetAllFreelancerAccountsAsync(It.IsAny<CancellationToken>()), Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Retrieving freelancer accounts page {query.PageNo}, size {query.PageSize}", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Retrieved {result.Items.Count} freelancer accounts", Times.Once());
    }

    [Fact]
    public async Task Handle_EmptyResult_ReturnsEmptyPaginatedResult()
    {
        // Arrange
        var query = new GetAllFreelancerAccountsQuery(PageNo: 1, PageSize: 10);
        var accounts = new List<FreelancerAccountModel>();
        _freelancerAccountsServiceMock.Setup(s => s.GetAllFreelancerAccountsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(accounts);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().BeEmpty();
        result.PageNo.Should().Be(query.PageNo);
        result.PageSize.Should().Be(query.PageSize);
        result.TotalCount.Should().Be(0);
        _freelancerAccountsServiceMock.Verify(s => s.GetAllFreelancerAccountsAsync(It.IsAny<CancellationToken>()), Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Retrieving freelancer accounts page {query.PageNo}, size {query.PageSize}", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Retrieved 0 freelancer accounts", Times.Once());
    }
}
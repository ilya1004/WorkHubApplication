using PaymentsService.Application.UseCases.AccountUseCases.Queries.GetAllEmployerAccounts;
using PaymentsService.Domain.Abstractions.AccountsServices;
using PaymentsService.Domain.Models;
using PaymentsService.Tests.UnitTests.Extensions;

namespace PaymentsService.Tests.UnitTests.Tests.UseCases.AccountUseCases.Queries;

public class GetAllEmployerAccountsQueryHandlerTests
{
    private readonly Mock<IEmployerAccountsService> _employerAccountsServiceMock;
    private readonly Mock<ILogger<GetAllEmployerAccountsQueryHandler>> _loggerMock;
    private readonly GetAllEmployerAccountsQueryHandler _handler;

    public GetAllEmployerAccountsQueryHandlerTests()
    {
        _employerAccountsServiceMock = new Mock<IEmployerAccountsService>();
        _loggerMock = new Mock<ILogger<GetAllEmployerAccountsQueryHandler>>();
        _handler = new GetAllEmployerAccountsQueryHandler(_employerAccountsServiceMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ReturnsPaginatedEmployerAccounts()
    {
        // Arrange
        var query = new GetAllEmployerAccountsQuery(PageNo: 2, PageSize: 5);
        var accounts = new List<EmployerAccountModel>
        {
            new() { Id = "acc_1", OwnerEmail = "employer1@example.com", Currency = "USD", Balance = 1000 },
            new() { Id = "acc_2", OwnerEmail = "employer2@example.com", Currency = "EUR", Balance = 2000 },
            new() { Id = "acc_3", OwnerEmail = "employer3@example.com", Currency = "USD", Balance = 3000 },
            new() { Id = "acc_4", OwnerEmail = "employer4@example.com", Currency = "GBP", Balance = 4000 },
            new() { Id = "acc_5", OwnerEmail = "employer5@example.com", Currency = "USD", Balance = 5000 },
            new() { Id = "acc_6", OwnerEmail = "employer6@example.com", Currency = "EUR", Balance = 6000 }
        };
        _employerAccountsServiceMock.Setup(s => s.GetAllEmployerAccountsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(accounts);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(1);
        result.Items[0].Should().BeEquivalentTo(accounts[5]);
        result.PageNo.Should().Be(query.PageNo);
        result.PageSize.Should().Be(query.PageSize);
        result.TotalCount.Should().Be(1);
        _employerAccountsServiceMock.Verify(s => s.GetAllEmployerAccountsAsync(It.IsAny<CancellationToken>()), Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Retrieving employer accounts page {query.PageNo}, size {query.PageSize}", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Retrieved {result.Items.Count} employer accounts", Times.Once());
    }

    [Fact]
    public async Task Handle_EmptyResult_ReturnsEmptyPaginatedResult()
    {
        // Arrange
        var query = new GetAllEmployerAccountsQuery(PageNo: 1, PageSize: 10);
        var accounts = new List<EmployerAccountModel>();
        _employerAccountsServiceMock.Setup(s => s.GetAllEmployerAccountsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(accounts);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().BeEmpty();
        result.PageNo.Should().Be(query.PageNo);
        result.PageSize.Should().Be(query.PageSize);
        result.TotalCount.Should().Be(0);
        _employerAccountsServiceMock.Verify(s => s.GetAllEmployerAccountsAsync(It.IsAny<CancellationToken>()), Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Retrieving employer accounts page {query.PageNo}, size {query.PageSize}", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Retrieved 0 employer accounts", Times.Once());
    }
}
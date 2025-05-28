using PaymentsService.Application.UseCases.PaymentsUseCases.Queries.GetAllEmployerPayments;
using PaymentsService.Application.UseCases.PaymentsUseCases.Queries.GetEmployerMyPaymentsQuery;
using PaymentsService.Domain.Abstractions.TransfersServices;
using PaymentsService.Domain.Models;
using PaymentsService.Tests.UnitTests.Extensions;

namespace PaymentsService.Tests.UnitTests.Tests.UseCases.PaymentsUseCases.Queries;

public class GetAllEmployerPaymentsQueryHandlerTests
{
    private readonly Mock<ITransfersService> _transfersServiceMock;
    private readonly Mock<ILogger<GetEmployerMyPaymentsQueryHandler>> _loggerMock;
    private readonly GetAllEmployerPaymentsQueryHandler _handler;

    public GetAllEmployerPaymentsQueryHandlerTests()
    {
        _transfersServiceMock = new Mock<ITransfersService>();
        _loggerMock = new Mock<ILogger<GetEmployerMyPaymentsQueryHandler>>();
        _handler = new GetAllEmployerPaymentsQueryHandler(_transfersServiceMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ReturnsPaginatedPayments()
    {
        // Arrange
        var query = new GetAllEmployerPaymentsQuery(PageNo: 2, PageSize: 2);
        var payments = new List<ChargeModel>
        {
            new() { Id = "ch_1", Amount = 1000, Currency = "USD", Captured = true, Status = "succeeded", PaymentMethod = "card" },
            new() { Id = "ch_2", Amount = 2000, Currency = "USD", Captured = false, Status = "pending", PaymentMethod = "card" },
            new() { Id = "ch_3", Amount = 3000, Currency = "EUR", Captured = true, Status = "succeeded", PaymentMethod = "card" },
            new() { Id = "ch_4", Amount = 4000, Currency = "GBP", Captured = true, Status = "succeeded", PaymentMethod = "card" }
        };
        _transfersServiceMock.Setup(s => s.GetAllEmployerPaymentsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(payments);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(2);
        result.Items.Should().BeEquivalentTo(payments.Skip(2).Take(2));
        result.PageNo.Should().Be(query.PageNo);
        result.PageSize.Should().Be(query.PageSize);
        result.TotalCount.Should().Be(2);
        _transfersServiceMock.Verify(s => s.GetAllEmployerPaymentsAsync(It.IsAny<CancellationToken>()), Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Retrieving payments by page {query.PageNo}, size {query.PageSize}", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Retrieved {result.Items.Count} payments", Times.Once());
    }

    [Fact]
    public async Task Handle_EmptyResult_ReturnsEmptyPaginatedResult()
    {
        // Arrange
        var query = new GetAllEmployerPaymentsQuery(PageNo: 1, PageSize: 10);
        var payments = new List<ChargeModel>();
        _transfersServiceMock.Setup(s => s.GetAllEmployerPaymentsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(payments);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().BeEmpty();
        result.PageNo.Should().Be(query.PageNo);
        result.PageSize.Should().Be(query.PageSize);
        result.TotalCount.Should().Be(0);
        _transfersServiceMock.Verify(s => s.GetAllEmployerPaymentsAsync(It.IsAny<CancellationToken>()), Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Retrieving payments by page {query.PageNo}, size {query.PageSize}", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Retrieved 0 payments", Times.Once());
    }
}
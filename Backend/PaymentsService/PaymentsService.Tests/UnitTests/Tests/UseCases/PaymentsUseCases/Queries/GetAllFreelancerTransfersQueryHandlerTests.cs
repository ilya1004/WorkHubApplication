using PaymentsService.Application.UseCases.PaymentsUseCases.Queries.GetAllFreelancerTransfers;
using PaymentsService.Application.UseCases.PaymentsUseCases.Queries.GetFreelancerMyTransfers;
using PaymentsService.Domain.Abstractions.TransfersServices;
using PaymentsService.Domain.Models;
using PaymentsService.Tests.UnitTests.Extensions;

namespace PaymentsService.Tests.UnitTests.Tests.UseCases.PaymentsUseCases.Queries;

public class GetAllFreelancerTransfersQueryHandlerTests
{
    private readonly Mock<ITransfersService> _transfersServiceMock;
    private readonly Mock<ILogger<GetFreelancerMyTransfersQueryHandler>> _loggerMock;
    private readonly GetAllFreelancerTransfersQueryHandler _handler;

    public GetAllFreelancerTransfersQueryHandlerTests()
    {
        _transfersServiceMock = new Mock<ITransfersService>();
        _loggerMock = new Mock<ILogger<GetFreelancerMyTransfersQueryHandler>>();
        _handler = new GetAllFreelancerTransfersQueryHandler(_transfersServiceMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ReturnsPaginatedTransfers()
    {
        // Arrange
        var query = new GetAllFreelancerTransfersQuery(PageNo: 2, PageSize: 2);
        var transfers = new List<TransferModel>
        {
            new() { Id = "tr_1", Amount = 500, Currency = "USD", TransferGroup = "group_1", Metadata = new Dictionary<string, string>() },
            new() { Id = "tr_2", Amount = 1000, Currency = "USD", TransferGroup = "group_2", Metadata = new Dictionary<string, string>() },
            new() { Id = "tr_3", Amount = 1500, Currency = "EUR", TransferGroup = "group_3", Metadata = new Dictionary<string, string>() },
            new() { Id = "tr_4", Amount = 2000, Currency = "GBP", TransferGroup = "group_4", Metadata = new Dictionary<string, string>() }
        };
        _transfersServiceMock.Setup(s => s.GetAllFreelancerTransfersAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(transfers);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(2);
        result.Items.Should().BeEquivalentTo(transfers.Skip(2).Take(2));
        result.PageNo.Should().Be(query.PageNo);
        result.PageSize.Should().Be(query.PageSize);
        result.TotalCount.Should().Be(2);
        _transfersServiceMock.Verify(s => s.GetAllFreelancerTransfersAsync(It.IsAny<CancellationToken>()), Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Retrieving transfers by page {query.PageNo}, size {query.PageSize}", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Retrieved {result.Items.Count} transfers", Times.Once());
    }

    [Fact]
    public async Task Handle_EmptyResult_ReturnsEmptyPaginatedResult()
    {
        // Arrange
        var query = new GetAllFreelancerTransfersQuery(PageNo: 1, PageSize: 10);
        var transfers = new List<TransferModel>();
        _transfersServiceMock.Setup(s => s.GetAllFreelancerTransfersAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(transfers);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().BeEmpty();
        result.PageNo.Should().Be(query.PageNo);
        result.PageSize.Should().Be(query.PageSize);
        result.TotalCount.Should().Be(0);
        _transfersServiceMock.Verify(s => s.GetAllFreelancerTransfersAsync(It.IsAny<CancellationToken>()), Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Retrieving transfers by page {query.PageNo}, size {query.PageSize}", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Retrieved 0 transfers", Times.Once());
    }
}
using PaymentsService.Application.UseCases.PaymentsUseCases.Queries.GetEmployerMyPaymentsQuery;
using PaymentsService.Domain.Abstractions.TransfersServices;
using PaymentsService.Domain.Abstractions.UserContext;
using PaymentsService.Domain.Models;
using PaymentsService.Tests.UnitTests.Extensions;

namespace PaymentsService.Tests.UnitTests.Tests.UseCases.PaymentsUseCases.Queries;

public class GetEmployerMyPaymentsQueryHandlerTests
{
    private readonly Mock<IUserContext> _userContextMock;
    private readonly Mock<ITransfersService> _transfersServiceMock;
    private readonly Mock<ILogger<GetEmployerMyPaymentsQueryHandler>> _loggerMock;
    private readonly GetEmployerMyPaymentsQueryHandler _handler;

    public GetEmployerMyPaymentsQueryHandlerTests()
    {
        _userContextMock = new Mock<IUserContext>();
        _transfersServiceMock = new Mock<ITransfersService>();
        _loggerMock = new Mock<ILogger<GetEmployerMyPaymentsQueryHandler>>();
        _handler = new GetEmployerMyPaymentsQueryHandler(_userContextMock.Object, _transfersServiceMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_WithProjectId_ReturnsPaginatedPayments()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var query = new GetEmployerMyPaymentsQuery(ProjectId: projectId, PageNo: 2, PageSize: 2);
        var payments = new List<ChargeModel>
        {
            new() { Id = "ch_1", Amount = 1000, Currency = "USD", Captured = true, Status = "succeeded", PaymentMethod = "card" },
            new() { Id = "ch_2", Amount = 2000, Currency = "USD", Captured = false, Status = "pending", PaymentMethod = "card" },
            new() { Id = "ch_3", Amount = 3000, Currency = "EUR", Captured = true, Status = "succeeded", PaymentMethod = "card" },
            new() { Id = "ch_4", Amount = 4000, Currency = "GBP", Captured = true, Status = "succeeded", PaymentMethod = "card" }
        };
        _userContextMock.Setup(uc => uc.GetUserId()).Returns(userId);
        _transfersServiceMock.Setup(s => s.GetEmployerPaymentsAsync(userId, projectId, It.IsAny<CancellationToken>()))
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
        _transfersServiceMock.Verify(s => s.GetEmployerPaymentsAsync(userId, projectId, It.IsAny<CancellationToken>()), Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Retrieving payments for employer {userId}, project {projectId}, page {query.PageNo}, size {query.PageSize}", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Retrieved {result.Items.Count} payments for employer {userId}, project {projectId}", Times.Once());
    }

    [Fact]
    public async Task Handle_WithoutProjectId_ReturnsPaginatedPayments()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var query = new GetEmployerMyPaymentsQuery(ProjectId: null, PageNo: 1, PageSize: 3);
        var payments = new List<ChargeModel>
        {
            new() { Id = "ch_1", Amount = 1000, Currency = "USD", Captured = true, Status = "succeeded", PaymentMethod = "card" },
            new() { Id = "ch_2", Amount = 2000, Currency = "USD", Captured = false, Status = "pending", PaymentMethod = "card" }
        };
        _userContextMock.Setup(uc => uc.GetUserId()).Returns(userId);
        _transfersServiceMock.Setup(s => s.GetEmployerPaymentsAsync(userId, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(payments);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(2);
        result.Items.Should().BeEquivalentTo(payments);
        result.PageNo.Should().Be(query.PageNo);
        result.PageSize.Should().Be(query.PageSize);
        result.TotalCount.Should().Be(2);
        _transfersServiceMock.Verify(s => s.GetEmployerPaymentsAsync(userId, null, It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task Handle_EmptyResult_ReturnsEmptyPaginatedResult()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var query = new GetEmployerMyPaymentsQuery(ProjectId: projectId, PageNo: 1, PageSize: 10);
        var payments = new List<ChargeModel>();
        _userContextMock.Setup(uc => uc.GetUserId()).Returns(userId);
        _transfersServiceMock.Setup(s => s.GetEmployerPaymentsAsync(userId, projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(payments);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().BeEmpty();
        result.PageNo.Should().Be(query.PageNo);
        result.PageSize.Should().Be(query.PageSize);
        result.TotalCount.Should().Be(0);
        _transfersServiceMock.Verify(s => s.GetEmployerPaymentsAsync(userId, projectId, It.IsAny<CancellationToken>()), Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Retrieving payments for employer {userId}, project {projectId}, page {query.PageNo}, size {query.PageSize}", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Retrieved 0 payments for employer {userId}, project {projectId}", Times.Once());
    }
}
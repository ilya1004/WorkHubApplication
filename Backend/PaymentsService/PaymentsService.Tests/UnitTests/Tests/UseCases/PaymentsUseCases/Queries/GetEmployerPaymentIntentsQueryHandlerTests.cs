using PaymentsService.Application.UseCases.PaymentsUseCases.Queries.GetEmployerMyPaymentsQuery;
using PaymentsService.Application.UseCases.PaymentsUseCases.Queries.GetEmployerPaymentIntents;
using PaymentsService.Domain.Abstractions.TransfersServices;
using PaymentsService.Domain.Abstractions.UserContext;
using PaymentsService.Domain.Models;
using PaymentsService.Tests.UnitTests.Extensions;

namespace PaymentsService.Tests.UnitTests.Tests.UseCases.PaymentsUseCases.Queries;

public class GetEmployerPaymentIntentsQueryHandlerTests
{
    private readonly Mock<IUserContext> _userContextMock;
    private readonly Mock<ITransfersService> _transfersServiceMock;
    private readonly Mock<ILogger<GetEmployerMyPaymentsQueryHandler>> _loggerMock;
    private readonly GetEmployerPaymentIntentsQueryHandler _handler;

    public GetEmployerPaymentIntentsQueryHandlerTests()
    {
        _userContextMock = new Mock<IUserContext>();
        _transfersServiceMock = new Mock<ITransfersService>();
        _loggerMock = new Mock<ILogger<GetEmployerMyPaymentsQueryHandler>>();
        _handler = new GetEmployerPaymentIntentsQueryHandler(_userContextMock.Object, _transfersServiceMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_WithProjectId_ReturnsPaginatedPaymentIntents()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var query = new GetEmployerPaymentIntentsQuery(ProjectId: projectId, PageNo: 2, PageSize: 2);
        var paymentIntents = new List<PaymentIntentModel>
        {
            new() { Id = "pi_1", Amount = 1000, Currency = "EUR", Status = "succeeded"},
            new() { Id = "pi_2", Amount = 2000, Currency = "EUR", Status = "succeeded"},
            new() { Id = "pi_3", Amount = 3000, Currency = "EUR", Status = "succeeded",},
            new() { Id = "pi_4", Amount = 4000, Currency = "EUR", Status = "succeeded",}
        };
        _userContextMock.Setup(uc => uc.GetUserId()).Returns(userId);
        _transfersServiceMock.Setup(s => s.GetEmployerPaymentIntentsAsync(userId, projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(paymentIntents);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(2);
        result.Items.Should().BeEquivalentTo(paymentIntents.Skip(2).Take(2));
        result.PageNo.Should().Be(query.PageNo);
        result.PageSize.Should().Be(query.PageSize);
        result.TotalCount.Should().Be(2);
        _transfersServiceMock.Verify(s => s.GetEmployerPaymentIntentsAsync(userId, projectId, It.IsAny<CancellationToken>()), Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Retrieving payment intents for employer {userId}, project {projectId}, page {query.PageNo}, size {query.PageSize}", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Retrieved {result.Items.Count} payment intents for employer {userId}, project {projectId}", Times.Once());
    }

    [Fact]
    public async Task Handle_WithoutProjectId_ReturnsPaginatedPaymentIntents()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var query = new GetEmployerPaymentIntentsQuery(ProjectId: null, PageNo: 1, PageSize: 3);
        var paymentIntents = new List<PaymentIntentModel>
        {
            new() { Id = "pi_1", Amount = 1000, Currency = "USD", Status = "succeeded"},
            new() { Id = "pi_2", Amount = 2000, Currency = "USD", Status = "succeeded"}
        };
        _userContextMock.Setup(uc => uc.GetUserId()).Returns(userId);
        _transfersServiceMock.Setup(s => s.GetEmployerPaymentIntentsAsync(userId, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(paymentIntents);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(2);
        result.Items.Should().BeEquivalentTo(paymentIntents);
        result.PageNo.Should().Be(query.PageNo);
        result.PageSize.Should().Be(query.PageSize);
        result.TotalCount.Should().Be(2);
        _transfersServiceMock.Verify(s => s.GetEmployerPaymentIntentsAsync(userId, null, It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task Handle_EmptyResult_ReturnsEmptyPaginatedResult()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var query = new GetEmployerPaymentIntentsQuery(ProjectId: projectId, PageNo: 1, PageSize: 10);
        var paymentIntents = new List<PaymentIntentModel>();
        _userContextMock.Setup(uc => uc.GetUserId()).Returns(userId);
        _transfersServiceMock.Setup(s => s.GetEmployerPaymentIntentsAsync(userId, projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(paymentIntents);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().BeEmpty();
        result.PageNo.Should().Be(query.PageNo);
        result.PageSize.Should().Be(query.PageSize);
        result.TotalCount.Should().Be(0);
        _transfersServiceMock.Verify(s => s.GetEmployerPaymentIntentsAsync(userId, projectId, It.IsAny<CancellationToken>()), Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Retrieving payment intents for employer {userId}, project {projectId}, page {query.PageNo}, size {query.PageSize}", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Retrieved 0 payment intents for employer {userId}, project {projectId}", Times.Once());
    }
}
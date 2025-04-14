using PaymentsService.Application.UseCases.PaymentMethodUseCases.Queries.GetMyPaymentMethods;
using PaymentsService.Domain.Abstractions.PaymentsServices;
using PaymentsService.Domain.Abstractions.UserContext;
using PaymentsService.Domain.Models;
using PaymentsService.Tests.UnitTests.Extensions;

namespace PaymentsService.Tests.UnitTests.Tests.UseCases.PaymentMethodUseCases.Queries;

public class GetMyPaymentMethodsQueryHandlerTests
{
    private readonly Mock<IPaymentMethodsService> _paymentMethodsServiceMock;
    private readonly Mock<IUserContext> _userContextMock;
    private readonly Mock<ILogger<GetMyPaymentMethodsQueryHandler>> _loggerMock;
    private readonly GetMyPaymentMethodsQueryHandler _handler;

    public GetMyPaymentMethodsQueryHandlerTests()
    {
        _paymentMethodsServiceMock = new Mock<IPaymentMethodsService>();
        _userContextMock = new Mock<IUserContext>();
        _loggerMock = new Mock<ILogger<GetMyPaymentMethodsQueryHandler>>();
        _handler = new GetMyPaymentMethodsQueryHandler(
            _paymentMethodsServiceMock.Object,
            _userContextMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ReturnsPaymentMethods()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var paymentMethods = new List<PaymentMethodModel>
        {
            new()
            {
                Id = "pm_123",
                Type = "card",
                Card = new CardModel { Brand = "visa", Last4Digits = "1234" },
                CreatedAt = DateTime.UtcNow.AddDays(-1)
            },
            new()
            {
                Id = "pm_456",
                Type = "card",
                Card = new CardModel { Brand = "mastercard", Last4Digits = "5678" },
                CreatedAt = DateTime.UtcNow
            }
        };
        var query = new GetMyPaymentMethodsQuery();
        _userContextMock.Setup(uc => uc.GetUserId()).Returns(userId);
        _paymentMethodsServiceMock.Setup(s => s.GetPaymentMethodsAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(paymentMethods);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(paymentMethods);
        _paymentMethodsServiceMock.Verify(s => s.GetPaymentMethodsAsync(userId, It.IsAny<CancellationToken>()), Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Retrieving payment methods for user {userId}", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Successfully retrieved payment methods for user {userId}", Times.Once());
    }

    [Fact]
    public async Task Handle_EmptyResult_ReturnsEmptyList()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var paymentMethods = new List<PaymentMethodModel>();
        var query = new GetMyPaymentMethodsQuery();
        _userContextMock.Setup(uc => uc.GetUserId()).Returns(userId);
        _paymentMethodsServiceMock.Setup(s => s.GetPaymentMethodsAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(paymentMethods);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeEmpty();
        _paymentMethodsServiceMock.Verify(s => s.GetPaymentMethodsAsync(userId, It.IsAny<CancellationToken>()), Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Retrieving payment methods for user {userId}", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Successfully retrieved payment methods for user {userId}", Times.Once());
    }
}
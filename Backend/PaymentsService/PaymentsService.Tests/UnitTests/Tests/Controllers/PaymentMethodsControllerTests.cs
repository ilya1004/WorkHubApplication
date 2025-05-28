using Microsoft.AspNetCore.Mvc;
using PaymentsService.API.Controllers;
using PaymentsService.Application.UseCases.PaymentMethodUseCases.Commands.DeletePaymentMethod;
using PaymentsService.Application.UseCases.PaymentMethodUseCases.Commands.SavePaymentMethod;
using PaymentsService.Application.UseCases.PaymentMethodUseCases.Queries.GetMyPaymentMethods;
using PaymentsService.Domain.Models;

namespace PaymentsService.Tests.UnitTests.Tests.Controllers;

public class PaymentMethodsControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly PaymentMethodsController _controller;

    public PaymentMethodsControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _controller = new PaymentMethodsController(_mediatorMock.Object);
    }

    [Fact]
    public async Task SavePaymentMethod_SendsCommand_ReturnsNoContent()
    {
        // Arrange
        var paymentMethodId = "pm_123";
        _mediatorMock.Setup(m => m.Send(
                It.Is<SavePaymentMethodCommand>(c => c.PaymentMethodId == paymentMethodId), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.SavePaymentMethod(paymentMethodId);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        var noContentResult = (NoContentResult)result;
        noContentResult.StatusCode.Should().Be(204);
        _mediatorMock.Verify(m => m.Send(
            It.Is<SavePaymentMethodCommand>(c => c.PaymentMethodId == paymentMethodId), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetMyPaymentMethods_ReturnsPaymentMethods()
    {
        // Arrange
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
        _mediatorMock.Setup(m => m.Send(It.IsAny<GetMyPaymentMethodsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(paymentMethods);

        // Act
        var result = await _controller.GetMyPaymentMethods();

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = (OkObjectResult)result;
        okResult.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(paymentMethods);
        _mediatorMock.Verify(m => m.Send(It.IsAny<GetMyPaymentMethodsQuery>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task DeletePaymentMethod_SendsCommand_ReturnsNoContent()
    {
        // Arrange
        var paymentMethodId = "pm_789";
        _mediatorMock.Setup(m => m.Send(
                It.Is<DeletePaymentMethodCommand>(c => c.PaymentMethodId == paymentMethodId), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.DeletePaymentMethod(paymentMethodId);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        var noContentResult = (NoContentResult)result;
        noContentResult.StatusCode.Should().Be(204);
        _mediatorMock.Verify(m => m.Send(
            It.Is<DeletePaymentMethodCommand>(c => c.PaymentMethodId == paymentMethodId), It.IsAny<CancellationToken>()), Times.Once());
    }
}
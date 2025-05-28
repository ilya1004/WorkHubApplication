using Microsoft.AspNetCore.Mvc;
using PaymentsService.API.Contracts.CommonContracts;
using PaymentsService.API.Contracts.PaymentContracts;
using PaymentsService.API.Controllers;
using PaymentsService.Application.UseCases.PaymentsUseCases.Commands.ConfirmPaymentForProject;
using PaymentsService.Application.UseCases.PaymentsUseCases.Commands.PayForProjectWithSavedMethod;
using PaymentsService.Application.UseCases.PaymentsUseCases.Queries.GetAllEmployerPayments;
using PaymentsService.Application.UseCases.PaymentsUseCases.Queries.GetAllFreelancerTransfers;
using PaymentsService.Application.UseCases.PaymentsUseCases.Queries.GetEmployerMyPaymentsQuery;
using PaymentsService.Application.UseCases.PaymentsUseCases.Queries.GetEmployerPaymentIntents;
using PaymentsService.Application.UseCases.PaymentsUseCases.Queries.GetFreelancerMyTransfers;
using PaymentsService.Domain.Models;

namespace PaymentsService.Tests.UnitTests.Tests.Controllers;

public class PaymentsControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly PaymentsController _controller;

    public PaymentsControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _mapperMock = new Mock<IMapper>();
        _controller = new PaymentsController(_mediatorMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task CreatePaymentByProject_SendsCommand_ReturnsNoContent()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var paymentMethodId = "pm_123";
        _mediatorMock.Setup(m => m.Send(It.Is<PayForProjectWithSavedMethodCommand>(c => c.ProjectId == projectId && c.PaymentMethodId == paymentMethodId), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.CreatePaymentByProject(projectId, paymentMethodId);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        var noContentResult = (NoContentResult)result;
        noContentResult.StatusCode.Should().Be(204);
        _mediatorMock.Verify(m => m.Send(It.Is<PayForProjectWithSavedMethodCommand>(c => c.ProjectId == projectId && c.PaymentMethodId == paymentMethodId), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task ConfirmPayment_SendsCommand_ReturnsNoContent()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        _mediatorMock.Setup(m => m.Send(It.Is<ConfirmPaymentForProjectCommand>(c => c.ProjectId == projectId), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.ConfirmPayment(projectId);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        var noContentResult = (NoContentResult)result;
        noContentResult.StatusCode.Should().Be(204);
        _mediatorMock.Verify(m => m.Send(It.Is<ConfirmPaymentForProjectCommand>(c => c.ProjectId == projectId), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetEmployerMyPayments_ReturnsPaginatedPayments()
    {
        // Arrange
        var request = new GetOperationsRequest(ProjectId: Guid.NewGuid(), PageNo: 1, PageSize: 10);
        var query = new GetEmployerMyPaymentsQuery(request.ProjectId, request.PageNo, request.PageSize);
        var payments = new PaginatedResultModel<ChargeModel>
        {
            Items = new List<ChargeModel>
            {
                new() { Id = "ch_1", Amount = 1000, Currency = "usd", Status = "succeeded", PaymentMethod = "card" },
                new() { Id = "ch_2", Amount = 2000, Currency = "usd", Status = "pending", PaymentMethod = "card" }
            },
            TotalCount = 50,
            PageNo = 1,
            PageSize = 10
        };
        _mapperMock.Setup(m => m.Map<GetEmployerMyPaymentsQuery>(request)).Returns(query);
        _mediatorMock.Setup(m => m.Send(query, It.IsAny<CancellationToken>()))
            .ReturnsAsync(payments);

        // Act
        var result = await _controller.GetEmployerMyPayments(request);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = (OkObjectResult)result;
        okResult.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(payments);
        _mapperMock.Verify(m => m.Map<GetEmployerMyPaymentsQuery>(request), Times.Once());
        _mediatorMock.Verify(m => m.Send(query, It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetAllEmployerPayments_ReturnsPaginatedPayments()
    {
        // Arrange
        var request = new GetPaginatedListRequest(PageNo: 2, PageSize: 5);
        var payments = new PaginatedResultModel<ChargeModel>
        {
            Items = new List<ChargeModel>
            {
                new() { Id = "ch_3", Amount = 3000, Currency = "eur", Status = "succeeded", PaymentMethod = "card" }
            },
            TotalCount = 30,
            PageNo = 2,
            PageSize = 5
        };
        _mediatorMock.Setup(m => m.Send(It.Is<GetAllEmployerPaymentsQuery>(q => q.PageNo == request.PageNo && q.PageSize == request.PageSize), It.IsAny<CancellationToken>()))
            .ReturnsAsync(payments);

        // Act
        var result = await _controller.GetAllEmployerPayments(request);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = (OkObjectResult)result;
        okResult.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(payments);
        _mediatorMock.Verify(m => m.Send(It.Is<GetAllEmployerPaymentsQuery>(q => q.PageNo == request.PageNo && q.PageSize == request.PageSize), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetEmployerMyPaymentIntents_ReturnsPaginatedIntents()
    {
        // Arrange
        var request = new GetOperationsRequest(ProjectId: null, PageNo: 1, PageSize: 20);
        var query = new GetEmployerPaymentIntentsQuery(request.ProjectId, request.PageNo, request.PageSize);
        var intents = new PaginatedResultModel<PaymentIntentModel>
        {
            Items = new List<PaymentIntentModel>
            {
                new() { Id = "pi_1", Amount = 1500, Currency = "usd", Status = "requires_confirmation", Created = DateTime.UtcNow },
                new() { Id = "pi_2", Amount = 2500, Currency = "usd", Status = "succeeded", Created = DateTime.UtcNow }
            },
            TotalCount = 40,
            PageNo = 1,
            PageSize = 20
        };
        _mapperMock.Setup(m => m.Map<GetEmployerPaymentIntentsQuery>(request)).Returns(query);
        _mediatorMock.Setup(m => m.Send(query, It.IsAny<CancellationToken>()))
            .ReturnsAsync(intents);

        // Act
        var result = await _controller.GetEmployerMyPaymentIntents(request);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = (OkObjectResult)result;
        okResult.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(intents);
        _mapperMock.Verify(m => m.Map<GetEmployerPaymentIntentsQuery>(request), Times.Once());
        _mediatorMock.Verify(m => m.Send(query, It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetFreelancerMyTransfers_ReturnsPaginatedTransfers()
    {
        // Arrange
        var request = new GetOperationsRequest(ProjectId: Guid.NewGuid(), PageNo: 3, PageSize: 15);
        var query = new GetFreelancerMyTransfersQuery(request.ProjectId, request.PageNo, request.PageSize);
        var transfers = new PaginatedResultModel<TransferModel>
        {
            Items = new List<TransferModel>
            {
                new() { Id = "tr_1", Amount = 500, Currency = "usd", TransferGroup = "group_1" },
                new() { Id = "tr_2", Amount = 1000, Currency = "usd", TransferGroup = "group_2" }
            },
            TotalCount = 45,
            PageNo = 3,
            PageSize = 15
        };
        _mapperMock.Setup(m => m.Map<GetFreelancerMyTransfersQuery>(request)).Returns(query);
        _mediatorMock.Setup(m => m.Send(query, It.IsAny<CancellationToken>()))
            .ReturnsAsync(transfers);

        // Act
        var result = await _controller.GetFreelancerMyTransfers(request);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = (OkObjectResult)result;
        okResult.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(transfers);
        _mapperMock.Verify(m => m.Map<GetFreelancerMyTransfersQuery>(request), Times.Once());
        _mediatorMock.Verify(m => m.Send(query, It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetAllFreelancerTransfers_ReturnsPaginatedTransfers()
    {
        // Arrange
        var request = new GetPaginatedListRequest(PageNo: 1, PageSize: 25);
        var transfers = new PaginatedResultModel<TransferModel>
        {
            Items = new List<TransferModel>
            {
                new() { Id = "tr_3", Amount = 750, Currency = "eur", TransferGroup = "group_3" }
            },
            TotalCount = 25,
            PageNo = 1,
            PageSize = 25
        };
        _mediatorMock.Setup(m => m.Send(It.Is<GetAllFreelancerTransfersQuery>(q => q.PageNo == request.PageNo && q.PageSize == request.PageSize), It.IsAny<CancellationToken>()))
            .ReturnsAsync(transfers);

        // Act
        var result = await _controller.GetAllFreelancerTransfers(request);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = (OkObjectResult)result;
        okResult.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(transfers);
        _mediatorMock.Verify(m => m.Send(It.Is<GetAllFreelancerTransfersQuery>(q => q.PageNo == request.PageNo && q.PageSize == request.PageSize), It.IsAny<CancellationToken>()), Times.Once());
    }
}
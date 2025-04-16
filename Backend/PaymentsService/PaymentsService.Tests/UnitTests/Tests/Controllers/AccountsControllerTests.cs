using Microsoft.AspNetCore.Mvc;
using PaymentsService.API.Contracts.CommonContracts;
using PaymentsService.API.Controllers;
using PaymentsService.Application.UseCases.AccountUseCases.Commands.CreateEmployerAccount;
using PaymentsService.Application.UseCases.AccountUseCases.Commands.CreateFreelancerAccount;
using PaymentsService.Application.UseCases.AccountUseCases.Queries.GetAllEmployerAccounts;
using PaymentsService.Application.UseCases.AccountUseCases.Queries.GetAllFreelancerAccounts;
using PaymentsService.Application.UseCases.AccountUseCases.Queries.GetEmployerAccount;
using PaymentsService.Application.UseCases.AccountUseCases.Queries.GetFreelancerAccount;
using PaymentsService.Domain.Models;

namespace PaymentsService.Tests.UnitTests.Tests.Controllers;

public class AccountsControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly AccountsController _controller;

    public AccountsControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _controller = new AccountsController(_mediatorMock.Object);
    }

    [Fact]
    public async Task CreateEmployerAccount_SendsCommand_ReturnsCreated()
    {
        // Arrange
        _mediatorMock.Setup(m => m.Send(It.IsAny<CreateEmployerAccountCommand>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.CreateEmployerAccount();

        // Assert
        result.Should().BeOfType<CreatedResult>();
        _mediatorMock.Verify(m => m.Send(It.IsAny<CreateEmployerAccountCommand>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task CreateFreelancerAccount_SendsCommand_ReturnsCreated()
    {
        // Arrange
        _mediatorMock.Setup(m => m.Send(It.IsAny<CreateFreelancerAccountCommand>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.CreateFreelancerAccount();

        // Assert
        result.Should().BeOfType<CreatedResult>();
        _mediatorMock.Verify(m => m.Send(It.IsAny<CreateFreelancerAccountCommand>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetEmployerAccount_ReturnsAccountDetails()
    {
        // Arrange
        var account = new EmployerAccountModel
        {
            Id = Guid.NewGuid().ToString(),
            OwnerEmail = "employer@example.com",
            Currency = "USD",
            Balance = 1000
        };
        _mediatorMock.Setup(m => m.Send(It.IsAny<GetEmployerAccountQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(account);

        // Act
        var result = await _controller.GetEmployerAccount();

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = (OkObjectResult)result;
        okResult.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(account);
        _mediatorMock.Verify(m => m.Send(It.IsAny<GetEmployerAccountQuery>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetFreelancerAccount_ReturnsAccountDetails()
    {
        // Arrange
        var account = new FreelancerAccountModel
        {
            Id = Guid.NewGuid().ToString(),
            OwnerEmail = "freelancer@example.com",
            AccountType = "individual",
            Country = "US",
            Balance = 500
        };
        _mediatorMock.Setup(m => m.Send(It.IsAny<GetFreelancerAccountQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(account);

        // Act
        var result = await _controller.GetFreelancerAccount();

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = (OkObjectResult)result;
        okResult.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(account);
        _mediatorMock.Verify(m => m.Send(It.IsAny<GetFreelancerAccountQuery>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetAllEmployerAccount_ReturnsPaginatedResult()
    {
        // Arrange
        var request = new GetPaginatedListRequest { PageNo = 1, PageSize = 10 };
        var accounts = new PaginatedResultModel<EmployerAccountModel>
        {
            Items = new List<EmployerAccountModel>
            {
                new() { Id = Guid.NewGuid().ToString(), OwnerEmail = "employer1@example.com", Currency = "USD", Balance = 1000 },
                new() { Id = Guid.NewGuid().ToString(), OwnerEmail = "employer2@example.com", Currency = "EUR", Balance = 2000 }
            },
            TotalCount = 50,
            PageNo = 1,
            PageSize = 10
        };
        _mediatorMock.Setup(m => m.Send(It.Is<GetAllEmployerAccountsQuery>(q => q.PageNo == request.PageNo && q.PageSize == request.PageSize), It.IsAny<CancellationToken>()))
            .ReturnsAsync(accounts);

        // Act
        var result = await _controller.GetAllEmployerAccount(request);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = (OkObjectResult)result;
        okResult.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(accounts);
        _mediatorMock.Verify(m => m.Send(It.Is<GetAllEmployerAccountsQuery>(q => q.PageNo == request.PageNo && q.PageSize == request.PageSize), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetAllFreelancersAccount_ReturnsPaginatedResult()
    {
        // Arrange
        var request = new GetPaginatedListRequest { PageNo = 2, PageSize = 5 };
        var accounts = new PaginatedResultModel<FreelancerAccountModel>
        {
            Items = new List<FreelancerAccountModel>
            {
                new() { Id = Guid.NewGuid().ToString(), OwnerEmail = "freelancer1@example.com", AccountType = "individual", Country = "US", Balance = 500 },
                new() { Id = Guid.NewGuid().ToString(), OwnerEmail = "freelancer2@example.com", AccountType = "business", Country = "UK", Balance = 1500 }
            },
            TotalCount = 30,
            PageNo = 2,
            PageSize = 5
        };
        _mediatorMock.Setup(m => m.Send(It.Is<GetAllFreelancerAccountsQuery>(q => q.PageNo == request.PageNo && q.PageSize == request.PageSize), It.IsAny<CancellationToken>()))
            .ReturnsAsync(accounts);

        // Act
        var result = await _controller.GetAllFreelancersAccount(request);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = (OkObjectResult)result;
        okResult.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(accounts);
        _mediatorMock.Verify(m => m.Send(It.Is<GetAllFreelancerAccountsQuery>(q => q.PageNo == request.PageNo && q.PageSize == request.PageSize), It.IsAny<CancellationToken>()), Times.Once());
    }
}
using PaymentsService.Application.Exceptions;
using PaymentsService.Domain.Models;
using PaymentsService.Infrastructure.DTOs;
using PaymentsService.Infrastructure.Interfaces;
using PaymentsService.Infrastructure.Services.StripeAccountsServices;
using PaymentsService.Tests.UnitTests.Extensions;
using Stripe;

namespace PaymentsService.Tests.UnitTests.Tests.Services.StripeServices;

public class StripeEmployerAccountsServiceTests
{
    private readonly Mock<IEmployersGrpcClient> _employersGrpcClientMock;
    private readonly Mock<ILogger<StripeEmployerAccountsService>> _loggerMock;
    private readonly Mock<CustomerService> _customerServiceMock;
    private readonly StripeEmployerAccountsService _service;

    public StripeEmployerAccountsServiceTests()
    {
        _employersGrpcClientMock = new Mock<IEmployersGrpcClient>();
        _loggerMock = new Mock<ILogger<StripeEmployerAccountsService>>();
        _customerServiceMock = new Mock<CustomerService>();
        _service = new StripeEmployerAccountsService(_employersGrpcClientMock.Object, _loggerMock.Object);
        var field = typeof(StripeEmployerAccountsService).GetField("_customerService", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        field!.SetValue(_service, _customerServiceMock.Object);
    }

    [Fact]
    public async Task CreateEmployerAccountAsync_NewUser_CreatesAccountAndReturnsCustomerId()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var email = "test@example.com";
        var employerDto = new EmployerDto { Id = userId.ToString(), EmployerCustomerId = null };
        var customer = new Customer { Id = "cus_123", Email = email };
        _employersGrpcClientMock.Setup(c => c.GetEmployerByIdAsync(userId.ToString(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(employerDto);
        _customerServiceMock.Setup(s => s.CreateAsync(It.Is<CustomerCreateOptions>(
                o => o.Email == email && o.Metadata["UserId"] == userId.ToString()), null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(customer);

        // Act
        var result = await _service.CreateEmployerAccountAsync(userId, email, CancellationToken.None);

        // Assert
        result.Should().Be("cus_123");
        _employersGrpcClientMock.Verify(c => c.GetEmployerByIdAsync(userId.ToString(), It.IsAny<CancellationToken>()), Times.Once());
        _customerServiceMock.Verify(s => s.CreateAsync(It.IsAny<CustomerCreateOptions>(), null, It.IsAny<CancellationToken>()), Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Creating Stripe employer account for user {userId} with email {email}", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Successfully created Stripe customer {customer.Id} for user {userId}", Times.Once());
    }

    [Fact]
    public async Task CreateEmployerAccountAsync_ExistingAccount_ThrowsAlreadyExistsException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var email = "test@example.com";
        var employerDto = new EmployerDto { Id = userId.ToString(), EmployerCustomerId = "cus_123" };
        _employersGrpcClientMock.Setup(c => c.GetEmployerByIdAsync(userId.ToString(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(employerDto);

        // Act
        var act = async () => await _service.CreateEmployerAccountAsync(userId, email, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<AlreadyExistsException>().WithMessage("Your account already exists.");
        _employersGrpcClientMock.Verify(c => c.GetEmployerByIdAsync(userId.ToString(), It.IsAny<CancellationToken>()), Times.Once());
        _customerServiceMock.Verify(s => s.CreateAsync(It.IsAny<CustomerCreateOptions>(), null, It.IsAny<CancellationToken>()), Times.Never());
        _loggerMock.VerifyLog(LogLevel.Information, $"Creating Stripe employer account for user {userId} with email {email}", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Warning, $"Employer account already exists for user {userId}", Times.Once());
    }

    [Fact]
    public async Task CreateEmployerAccountAsync_StripeException_ThrowsBadRequestException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var email = "test@example.com";
        var employerDto = new EmployerDto { Id = userId.ToString(), EmployerCustomerId = null };
        var stripeException = new StripeException("Stripe error");
        _employersGrpcClientMock.Setup(c => c.GetEmployerByIdAsync(userId.ToString(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(employerDto);
        _customerServiceMock.Setup(s => s.CreateAsync(It.IsAny<CustomerCreateOptions>(), null, It.IsAny<CancellationToken>()))
            .ThrowsAsync(stripeException);

        // Act
        var act = async () => await _service.CreateEmployerAccountAsync(userId, email, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<BadRequestException>().WithMessage("Stripe error: Stripe error");
        _loggerMock.VerifyLog(LogLevel.Error, $"Stripe error while creating account for user {userId}: {stripeException.Message}", Times.Once());
    }

    [Fact]
    public async Task CreateEmployerAccountAsync_CustomerNull_ThrowsBadRequestException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var email = "test@example.com";
        var employerDto = new EmployerDto { Id = userId.ToString(), EmployerCustomerId = null };
        _employersGrpcClientMock.Setup(c => c.GetEmployerByIdAsync(userId.ToString(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(employerDto);
        _customerServiceMock.Setup(s => s.CreateAsync(It.IsAny<CustomerCreateOptions>(), null, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Customer?)null);

        // Act
        var act = async () => await _service.CreateEmployerAccountAsync(userId, email, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<BadRequestException>().WithMessage($"Could not create an account for employer with ID '{userId}'.");
        _loggerMock.VerifyLog(LogLevel.Error, $"Failed to create Stripe customer for user {userId}", Times.Once());
    }

    [Fact]
    public async Task GetEmployerAccountAsync_ValidAccount_ReturnsEmployerAccountModel()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var employerDto = new EmployerDto { Id = userId.ToString(), EmployerCustomerId = "cus_123" };
        var customer = new Customer
        {
            Id = "cus_123",
            Email = "test@example.com",
            Currency = "usd",
            Balance = 1000
        };
        _employersGrpcClientMock.Setup(c => c.GetEmployerByIdAsync(userId.ToString(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(employerDto);
        _customerServiceMock.Setup(s => s.GetAsync("cus_123", null, It.IsAny<RequestOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(customer);

        // Act
        var result = await _service.GetEmployerAccountAsync(userId, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(new EmployerAccountModel
        {
            Id = "cus_123",
            OwnerEmail = "test@example.com",
            Currency = "usd",
            Balance = 1000
        });
        _employersGrpcClientMock.Verify(c => c.GetEmployerByIdAsync(userId.ToString(), It.IsAny<CancellationToken>()), Times.Once());
        _customerServiceMock.Verify(s => s.GetAsync("cus_123", null, It.IsAny<RequestOptions>(), It.IsAny<CancellationToken>()), Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Getting Stripe employer account for user {userId}", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Successfully retrieved Stripe account {customer.Id} for user {userId}", Times.Once());
    }

    [Fact]
    public async Task GetEmployerAccountAsync_NoCustomerId_ThrowsNotFoundException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var employerDto = new EmployerDto { Id = userId.ToString(), EmployerCustomerId = null };
        _employersGrpcClientMock.Setup(c => c.GetEmployerByIdAsync(userId.ToString(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(employerDto);

        // Act
        Func<Task> act = async () => await _service.GetEmployerAccountAsync(userId, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>().WithMessage($"Stripe account by user ID '{userId}' not found.");
        _customerServiceMock.Verify(s => 
            s.GetAsync(It.IsAny<string>(), null, It.IsAny<RequestOptions>(), It.IsAny<CancellationToken>()), Times.Never());
        _loggerMock.VerifyLog(LogLevel.Information, $"Getting Stripe employer account for user {userId}", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Warning, $"Stripe account not found for user {userId}", Times.Once());
    }

    [Fact]
    public async Task GetEmployerAccountAsync_CustomerNull_ThrowsNotFoundException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var employerDto = new EmployerDto { Id = userId.ToString(), EmployerCustomerId = "cus_123" };
        _employersGrpcClientMock.Setup(c => c.GetEmployerByIdAsync(userId.ToString(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(employerDto);
        _customerServiceMock.Setup(s => s.GetAsync("cus_123", null, It.IsAny<RequestOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Customer?)null);

        // Act
        var act = async () => await _service.GetEmployerAccountAsync(userId, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<BadRequestException>().WithMessage($"Error getting Stripe account with ID '{employerDto.EmployerCustomerId}'.");
        _loggerMock.VerifyLog(LogLevel.Error, $"Stripe customer {employerDto.EmployerCustomerId} not found for user {userId}", Times.Once());
    }

    [Fact]
    public async Task GetEmployerAccountAsync_StripeException_ThrowsBadRequestException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var employerDto = new EmployerDto { Id = userId.ToString(), EmployerCustomerId = "cus_123" };
        var stripeException = new StripeException("Stripe error");
        _employersGrpcClientMock.Setup(c => c.GetEmployerByIdAsync(userId.ToString(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(employerDto);
        _customerServiceMock.Setup(s => s.GetAsync("cus_123", null, It.IsAny<RequestOptions>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(stripeException);

        // Act
        Func<Task> act = async () => await _service.GetEmployerAccountAsync(userId, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<BadRequestException>().WithMessage("Stripe error: Stripe error");
        _loggerMock.VerifyLog(LogLevel.Error, 
            $"Stripe error while getting account {employerDto.EmployerCustomerId}: {stripeException.Message}", Times.Once());
    }

    [Fact]
    public async Task GetAllEmployerAccountsAsync_ValidCustomers_ReturnsAccountList()
    {
        // Arrange
        var customers = new StripeList<Customer>
        {
            Data = new List<Customer>
            {
                new Customer { Id = "cus_123", Email = "test1@example.com", Currency = "usd", Balance = 1000 },
                new Customer { Id = "cus_456", Email = "test2@example.com", Currency = "eur", Balance = 2000 }
            }
        };
        _customerServiceMock.Setup(s => s.ListAsync(It.Is<CustomerListOptions>(o => o.Limit == 100), null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(customers);

        // Act
        var result = await _service.GetAllEmployerAccountsAsync(CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(new List<EmployerAccountModel>
        {
            new EmployerAccountModel { Id = "cus_123", OwnerEmail = "test1@example.com", Currency = "usd", Balance = 1000 },
            new EmployerAccountModel { Id = "cus_456", OwnerEmail = "test2@example.com", Currency = "eur", Balance = 2000 }
        });
        _customerServiceMock.Verify(s => s.ListAsync(It.IsAny<CustomerListOptions>(), null, It.IsAny<CancellationToken>()), Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, "Retrieving all Stripe employer accounts", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, "Successfully retrieved 2 employer accounts", Times.Once());
    }

    [Fact]
    public async Task GetAllEmployerAccountsAsync_EmptyList_ReturnsEmptyList()
    {
        // Arrange
        var customers = new StripeList<Customer> { Data = new List<Customer>() };
        _customerServiceMock.Setup(s => s.ListAsync(It.Is<CustomerListOptions>(o => o.Limit == 100), null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(customers);

        // Act
        var result = await _service.GetAllEmployerAccountsAsync(CancellationToken.None);

        // Assert
        result.Should().BeEmpty();
        _loggerMock.VerifyLog(LogLevel.Information, "Successfully retrieved 0 employer accounts", Times.Once());
    }

    [Fact]
    public async Task GetAllEmployerAccountsAsync_StripeException_ThrowsBadRequestException()
    {
        // Arrange
        var stripeException = new StripeException("Stripe error");
        _customerServiceMock.Setup(s => s.ListAsync(It.IsAny<CustomerListOptions>(), null, It.IsAny<CancellationToken>()))
            .ThrowsAsync(stripeException);

        // Act
        Func<Task> act = async () => await _service.GetAllEmployerAccountsAsync(CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<BadRequestException>().WithMessage("Stripe error: Stripe error");
        _loggerMock.VerifyLog(LogLevel.Error, $"Stripe error while retrieving employer accounts: {stripeException.Message}", Times.Once());
    }
}
using Microsoft.Extensions.DependencyInjection;
using PaymentsService.Application.Constants;
using PaymentsService.Application.Exceptions;
using PaymentsService.Domain.Abstractions.AccountsServices;
using PaymentsService.Domain.Models;
using PaymentsService.Infrastructure.DTOs;
using PaymentsService.Tests.IntegrationTests.Helpers;
using Stripe;

namespace PaymentsService.Tests.IntegrationTests.Tests.StripeServices;

public class StripeEmployerAccountsServiceIntegrationTests : IClassFixture<StripeIntegrationTestsFixture>
{
    private readonly StripeIntegrationTestsFixture _fixture;

    public StripeEmployerAccountsServiceIntegrationTests(StripeIntegrationTestsFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task CreateEmployerAccountAsync_NewAccount_ShouldCreateAndReturnCustomerId()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var email = "employer@example.com";
        var employerDto = new EmployerDto { Id = userId.ToString(), EmployerCustomerId = null };

        _fixture.EmployersGrpcClientMock
            .Setup(c => c.GetEmployerByIdAsync(userId.ToString(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(employerDto);

        using var scope = _fixture.Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IEmployerAccountsService>();

        // Act
        var customerId = await service.CreateEmployerAccountAsync(userId, email, CancellationToken.None);

        // Assert
        customerId.Should().NotBeNullOrEmpty();
        var customerService = new CustomerService();
        var customer = await customerService.GetAsync(customerId);
        customer.Should().NotBeNull();
        customer!.Email.Should().Be(email);
        customer.Metadata.Should().ContainKey("UserId").WhoseValue.Should().Be(userId.ToString());
        customer.Metadata.Should().ContainKey("Role").WhoseValue.Should().Be(AppRoles.EmployerRole);

        await customerService.DeleteAsync(customerId);
    }

    [Fact]
    public async Task CreateEmployerAccountAsync_ExistingAccount_ShouldThrowAlreadyExistsException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var email = "employer@example.com";
        var employerDto = new EmployerDto { Id = userId.ToString(), EmployerCustomerId = "cus_existing" };

        _fixture.EmployersGrpcClientMock
            .Setup(c => c.GetEmployerByIdAsync(userId.ToString(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(employerDto);

        using var scope = _fixture.Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IEmployerAccountsService>();

        // Act
        Func<Task> act = async () => await service.CreateEmployerAccountAsync(userId, email, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<AlreadyExistsException>()
            .WithMessage("Your account already exists.");
    }

    [Fact]
    public async Task GetEmployerAccountAsync_ExistingAccount_ShouldReturnAccountModel()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var customerService = new CustomerService();
        var customerOptions = new CustomerCreateOptions
        {
            Email = "employer@example.com",
            Metadata = new Dictionary<string, string> { { "UserId", userId.ToString() } }
        };
        var customer = await customerService.CreateAsync(customerOptions);
        var employerDto = new EmployerDto { Id = userId.ToString(), EmployerCustomerId = customer.Id };

        _fixture.EmployersGrpcClientMock
            .Setup(c => c.GetEmployerByIdAsync(userId.ToString(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(employerDto);

        using var scope = _fixture.Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IEmployerAccountsService>();

        // Act
        var accountModel = await service.GetEmployerAccountAsync(userId, CancellationToken.None);

        // Assert
        accountModel.Should().NotBeNull();
        accountModel.Id.Should().Be(customer.Id);
        accountModel.OwnerEmail.Should().Be(customer.Email);
        accountModel.Currency.Should().Be(customer.Currency);
        accountModel.Balance.Should().Be(customer.Balance);

        await customerService.DeleteAsync(customer.Id);
    }

    [Fact]
    public async Task GetEmployerAccountAsync_NonExistentAccount_ShouldThrowNotFoundException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var employerDto = new EmployerDto { Id = userId.ToString(), EmployerCustomerId = null };

        _fixture.EmployersGrpcClientMock
            .Setup(c => c.GetEmployerByIdAsync(userId.ToString(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(employerDto);

        using var scope = _fixture.Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IEmployerAccountsService>();

        // Act
        Func<Task> act = async () => await service.GetEmployerAccountAsync(userId, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Stripe account by user ID '{userId}' not found.");
    }

    [Fact]
    public async Task GetAllEmployerAccountsAsync_MultipleAccounts_ShouldReturnAllAccounts()
    {
        // Arrange
        var customerService = new CustomerService();
        var customers = new List<Customer>();
        for (int i = 0; i < 3; i++)
        {
            var options = new CustomerCreateOptions
            {
                Email = $"employer{i}@example.com",
                Metadata = new Dictionary<string, string>
                {
                    { "UserId", Guid.NewGuid().ToString() },
                    { "Role", AppRoles.EmployerRole }
                }
            };
            var customer = await customerService.CreateAsync(options);
            customers.Add(customer);
        }

        using var scope = _fixture.Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IEmployerAccountsService>();

        // Act
        var accounts = await service.GetAllEmployerAccountsAsync(CancellationToken.None);

        // Assert
        var employerAccountModels = accounts.ToList();
        employerAccountModels.Should().HaveCountGreaterThanOrEqualTo(customers.Count);
        foreach (var customer in customers)
        {
            employerAccountModels.Should().Contain(a =>
                a.Id == customer.Id &&
                a.OwnerEmail == customer.Email &&
                a.Currency == customer.Currency &&
                a.Balance == customer.Balance);
        }

        foreach (var customer in customers)
        {
            await customerService.DeleteAsync(customer.Id);
        }
    }

    [Fact]
    public async Task GetAllEmployerAccountsAsync_NoAccounts_ShouldReturnEmptyList()
    {
        // Arrange
        using var scope = _fixture.Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IEmployerAccountsService>();

        // Act
        var accounts = await service.GetAllEmployerAccountsAsync(CancellationToken.None);

        // Assert
        var employerAccountModels = accounts.ToList();
        employerAccountModels.Should().NotBeNull();
        employerAccountModels.Should().BeAssignableTo<IEnumerable<EmployerAccountModel>>();
    }
}
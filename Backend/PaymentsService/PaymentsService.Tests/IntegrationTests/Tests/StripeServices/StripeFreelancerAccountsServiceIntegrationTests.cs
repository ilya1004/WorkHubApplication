using Microsoft.Extensions.DependencyInjection;
using PaymentsService.Application.Constants;
using PaymentsService.Application.Exceptions;
using PaymentsService.Domain.Abstractions.AccountsServices;
using PaymentsService.Domain.Models;
using PaymentsService.Infrastructure.DTOs;
using PaymentsService.Tests.IntegrationTests.Helpers;
using Stripe;

namespace PaymentsService.Tests.IntegrationTests.Tests.StripeServices;

public class StripeFreelancerAccountsServiceIntegrationTests(
    StripeIntegrationTestsFixture fixture) : IClassFixture<StripeIntegrationTestsFixture>
{
    [Fact]
    public async Task CreateFreelancerAccountAsync_NewAccount_ShouldCreateAndReturnAccountId()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var email = "freelancer@example.com";
        var freelancerDto = new FreelancerDto { Id = userId.ToString(), StripeAccountId = null };

        fixture.FreelancersGrpcClientMock
            .Setup(c => c.GetFreelancerByIdAsync(userId.ToString(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(freelancerDto);

        using var scope = fixture.Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IFreelancerAccountsService>();

        // Act
        var accountId = await service.CreateFreelancerAccountAsync(userId, email, CancellationToken.None);

        // Assert
        accountId.Should().NotBeNullOrEmpty();
        var accountService = new AccountService();
        var account = await accountService.GetAsync(accountId);
        account.Should().NotBeNull();
        account!.Email.Should().Be(email);
        account.Type.Should().Be("custom");
        account.Country.Should().Be("LT");
        account.Metadata.Should().ContainKey("UserId").WhoseValue.Should().Be(userId.ToString());
        account.Metadata.Should().ContainKey("Role").WhoseValue.Should().Be(AppRoles.FreelancerRole);
        await accountService.DeleteAsync(accountId);
    }

    [Fact]
    public async Task CreateFreelancerAccountAsync_ExistingAccount_ShouldThrowAlreadyExistsException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var email = "freelancer@example.com";
        var freelancerDto = new FreelancerDto { Id = userId.ToString(), StripeAccountId = "acct_existing" };

        fixture.FreelancersGrpcClientMock
            .Setup(c => c.GetFreelancerByIdAsync(userId.ToString(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(freelancerDto);

        using var scope = fixture.Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IFreelancerAccountsService>();

        // Act
        Func<Task> act = async () => await service.CreateFreelancerAccountAsync(userId, email, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<AlreadyExistsException>()
            .WithMessage("Your account already exists.");
    }
    
    [Fact]
    public async Task GetFreelancerAccountAsync_NonExistentAccount_ShouldThrowNotFoundException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var freelancerDto = new FreelancerDto { Id = userId.ToString(), StripeAccountId = null };

        fixture.FreelancersGrpcClientMock
            .Setup(c => c.GetFreelancerByIdAsync(userId.ToString(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(freelancerDto);

        using var scope = fixture.Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IFreelancerAccountsService>();

        // Act
        var act = async () => await service.GetFreelancerAccountAsync(userId, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Stripe account with user ID '{userId}' not found.");
    }

    [Fact]
    public async Task GetAllFreelancerAccountsAsync_MultipleAccounts_ShouldThrowStripeExeption()
    {
        // Arrange
        var accountService = new AccountService();
        var accounts = new List<Account>();
        using var scope = fixture.Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IFreelancerAccountsService>();

        // Act
        var accountModels = await service.GetAllFreelancerAccountsAsync(CancellationToken.None);

        // Assert
        var freelancerAccountModels = accountModels.ToList();
        freelancerAccountModels.Should().HaveCountGreaterThanOrEqualTo(accounts.Count);
        foreach (var account in accounts)
        {
            freelancerAccountModels.Should().Contain(a =>
                a.Id == account.Id &&
                a.OwnerEmail == account.Email &&
                a.AccountType == account.Type &&
                a.Country == account.Country);
        }
        foreach (var account in accounts)
        {
            await accountService.DeleteAsync(account.Id);
        }
    }

    [Fact]
    public async Task GetAllFreelancerAccountsAsync_NoAccounts_ShouldReturnEmptyList()
    {
        // Arrange
        using var scope = fixture.Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IFreelancerAccountsService>();

        // Act
        var accountModels = await service.GetAllFreelancerAccountsAsync(CancellationToken.None);

        // Assert
        var freelancerAccountModels = accountModels.ToList();
        freelancerAccountModels.Should().NotBeNull();
        freelancerAccountModels.Should().BeAssignableTo<IEnumerable<FreelancerAccountModel>>();
    }
}
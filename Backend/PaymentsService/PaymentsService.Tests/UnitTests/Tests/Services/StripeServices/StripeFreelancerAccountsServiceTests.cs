using PaymentsService.Application.Exceptions;
using PaymentsService.Domain.Models;
using PaymentsService.Infrastructure.DTOs;
using PaymentsService.Infrastructure.Interfaces;
using PaymentsService.Infrastructure.Services.StripeAccountsServices;
using PaymentsService.Tests.UnitTests.Extensions;
using Stripe;

namespace PaymentsService.Tests.UnitTests.Tests.Services.StripeServices;

public class StripeFreelancerAccountsServiceTests
{
    private readonly Mock<IFreelancersGrpcClient> _freelancersGrpcClientMock;
    private readonly Mock<ILogger<StripeFreelancerAccountsService>> _loggerMock;
    private readonly Mock<AccountService> _accountServiceMock;
    private readonly Mock<BalanceService> _balanceServiceMock;
    private readonly StripeFreelancerAccountsService _service;

    public StripeFreelancerAccountsServiceTests()
    {
        _freelancersGrpcClientMock = new Mock<IFreelancersGrpcClient>();
        _loggerMock = new Mock<ILogger<StripeFreelancerAccountsService>>();
        _accountServiceMock = new Mock<AccountService>();
        _balanceServiceMock = new Mock<BalanceService>();
        _service = new StripeFreelancerAccountsService(_freelancersGrpcClientMock.Object, _loggerMock.Object);
        
        var accountServiceField = typeof(StripeFreelancerAccountsService)
            .GetField("_accountService", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var balanceServiceField = typeof(StripeFreelancerAccountsService)
            .GetField("_balanceService", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        accountServiceField!.SetValue(_service, _accountServiceMock.Object);
        balanceServiceField!.SetValue(_service, _balanceServiceMock.Object);
    }

    [Fact]
    public async Task CreateFreelancerAccountAsync_NewUser_CreatesAccountAndReturnsAccountId()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var email = "freelancer@example.com";
        var freelancerDto = new FreelancerDto { Id = userId.ToString(), StripeAccountId = null };
        var account = new Account { Id = "acct_123", Email = email, Type = "custom", Country = "LT" };
        _freelancersGrpcClientMock.Setup(c => c.GetFreelancerByIdAsync(userId.ToString(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(freelancerDto);
        _accountServiceMock.Setup(s => s.CreateAsync(It.Is<AccountCreateOptions>(o => 
            o.Email == email && 
            o.Metadata["UserId"] == userId.ToString() && 
            o.Type == "custom" && 
            o.Country == "LT"), null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(account);

        // Act
        var result = await _service.CreateFreelancerAccountAsync(userId, email, CancellationToken.None);

        // Assert
        result.Should().Be("acct_123");
        _freelancersGrpcClientMock.Verify(c => c.GetFreelancerByIdAsync(userId.ToString(), It.IsAny<CancellationToken>()), Times.Once());
        _accountServiceMock.Verify(s => s.CreateAsync(It.IsAny<AccountCreateOptions>(), null, It.IsAny<CancellationToken>()), Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Creating Stripe freelancer account for user {userId} with email {email}", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Successfully created Stripe account {account.Id} for user {userId}", Times.Once());
    }

    [Fact]
    public async Task CreateFreelancerAccountAsync_ExistingAccount_ThrowsAlreadyExistsException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var email = "freelancer@example.com";
        var freelancerDto = new FreelancerDto { Id = userId.ToString(), StripeAccountId = "acct_123" };
        _freelancersGrpcClientMock.Setup(c => c.GetFreelancerByIdAsync(userId.ToString(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(freelancerDto);

        // Act
        Func<Task> act = async () => await _service.CreateFreelancerAccountAsync(userId, email, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<AlreadyExistsException>().WithMessage("Your account already exists.");
        _freelancersGrpcClientMock.Verify(c => c.GetFreelancerByIdAsync(userId.ToString(), It.IsAny<CancellationToken>()), Times.Once());
        _accountServiceMock.Verify(s => s.CreateAsync(It.IsAny<AccountCreateOptions>(), null, It.IsAny<CancellationToken>()), Times.Never());
        _loggerMock.VerifyLog(LogLevel.Information, $"Creating Stripe freelancer account for user {userId} with email {email}", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Warning, $"Freelancer account already exists for user {userId}", Times.Once());
    }

    [Fact]
    public async Task CreateFreelancerAccountAsync_StripeException_ThrowsBadRequestException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var email = "freelancer@example.com";
        var freelancerDto = new FreelancerDto { Id = userId.ToString(), StripeAccountId = null };
        var stripeException = new StripeException("Stripe error");
        _freelancersGrpcClientMock.Setup(c => c.GetFreelancerByIdAsync(userId.ToString(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(freelancerDto);
        _accountServiceMock.Setup(s => s.CreateAsync(It.IsAny<AccountCreateOptions>(), null, It.IsAny<CancellationToken>()))
            .ThrowsAsync(stripeException);

        // Act
        Func<Task> act = async () => await _service.CreateFreelancerAccountAsync(userId, email, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<BadRequestException>().WithMessage("Stripe error: Stripe error");
        _loggerMock.VerifyLog(LogLevel.Error, $"Stripe error while creating account for user {userId}: {stripeException.Message}", Times.Once());
    }

    [Fact]
    public async Task CreateFreelancerAccountAsync_AccountNull_ThrowsBadRequestException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var email = "freelancer@example.com";
        var freelancerDto = new FreelancerDto { Id = userId.ToString(), StripeAccountId = null };
        _freelancersGrpcClientMock.Setup(c => c.GetFreelancerByIdAsync(userId.ToString(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(freelancerDto);
        _accountServiceMock.Setup(s => s.CreateAsync(It.IsAny<AccountCreateOptions>(), null, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Account?)null);

        // Act
        Func<Task> act = async () => await _service.CreateFreelancerAccountAsync(userId, email, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<BadRequestException>().WithMessage($"Could not create an account for freelancer with ID '{freelancerDto.Id}'. " +
                                                                         $"Error: Stripe account by user ID '{freelancerDto.Id}' is not created.");
        _loggerMock.VerifyLog(LogLevel.Error, $"Failed to create Stripe account for user {userId}", Times.Once());
    }

    [Fact]
    public async Task GetFreelancerAccountAsync_ValidAccount_ReturnsFreelancerAccountModel()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var freelancerDto = new FreelancerDto { Id = userId.ToString(), StripeAccountId = "acct_123" };
        var account = new Account { Id = "acct_123", Email = "freelancer@example.com", Type = "custom", Country = "LT" };
        var balance = new Balance
        {
            Available = new List<BalanceAmount>
            {
                new BalanceAmount { Amount = 1000, Currency = "eur" },
                new BalanceAmount { Amount = 500, Currency = "usd" }
            }
        };
        _freelancersGrpcClientMock.Setup(c => c.GetFreelancerByIdAsync(userId.ToString(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(freelancerDto);
        _accountServiceMock.Setup(s => s.GetAsync("acct_123", null, It.IsAny<RequestOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(account);
        _balanceServiceMock.Setup(s => s.GetAsync(It.IsAny<BalanceGetOptions>(), It.Is<RequestOptions>(o => o.StripeAccount == "acct_123"), It.IsAny<CancellationToken>()))
            .ReturnsAsync(balance);

        // Act
        var result = await _service.GetFreelancerAccountAsync(userId, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(new FreelancerAccountModel
        {
            Id = "acct_123",
            OwnerEmail = "freelancer@example.com",
            AccountType = "custom",
            Country = "LT",
            Balance = 1000
        });
        _freelancersGrpcClientMock.Verify(c => c.GetFreelancerByIdAsync(userId.ToString(), It.IsAny<CancellationToken>()), Times.Once());
        _accountServiceMock.Verify(s => s.GetAsync("acct_123", null, It.IsAny<RequestOptions>(), It.IsAny<CancellationToken>()), Times.Once());
        _balanceServiceMock.Verify(s => s.GetAsync(It.IsAny<BalanceGetOptions>(), It.IsAny<RequestOptions>(), It.IsAny<CancellationToken>()), Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Getting Stripe freelancer account for user {userId}", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Retrieving balance for account {freelancerDto.StripeAccountId}", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Successfully retrieved Stripe account {account.Id} for user {userId}", Times.Once());
    }

    [Fact]
    public async Task GetFreelancerAccountAsync_AccountNull_ThrowsNotFoundException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var freelancerDto = new FreelancerDto { Id = userId.ToString(), StripeAccountId = "acct_123" };
        _freelancersGrpcClientMock.Setup(c => c.GetFreelancerByIdAsync(userId.ToString(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(freelancerDto);
        _accountServiceMock.Setup(s => s.GetAsync("acct_123", null, It.IsAny<RequestOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Account?)null);

        // Act
        Func<Task> act = async () => await _service.GetFreelancerAccountAsync(userId, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<BadRequestException>().WithMessage($"Error getting Stripe account {freelancerDto.StripeAccountId}");
        _loggerMock.VerifyLog(LogLevel.Error, $"Stripe account or balance not found for account {freelancerDto.StripeAccountId}", Times.Once());
    }

    [Fact]
    public async Task GetFreelancerAccountAsync_BalanceNull_ThrowsNotFoundException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var freelancerDto = new FreelancerDto { Id = userId.ToString(), StripeAccountId = "acct_123" };
        var account = new Account { Id = "acct_123", Email = "freelancer@example.com", Type = "custom", Country = "LT" };
        _freelancersGrpcClientMock.Setup(c => c.GetFreelancerByIdAsync(userId.ToString(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(freelancerDto);
        _accountServiceMock.Setup(s => s.GetAsync("acct_123", null, It.IsAny<RequestOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(account);
        _balanceServiceMock.Setup(s => s.GetAsync(It.IsAny<BalanceGetOptions>(), It.IsAny<RequestOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Balance?)null);

        // Act
        Func<Task> act = async () => await _service.GetFreelancerAccountAsync(userId, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<BadRequestException>().WithMessage($"Error getting Stripe account {freelancerDto.StripeAccountId}");
        _loggerMock.VerifyLog(LogLevel.Error, $"Stripe account or balance not found for account {freelancerDto.StripeAccountId}", Times.Once());
    }

    [Fact]
    public async Task GetFreelancerAccountAsync_StripeException_ThrowsBadRequestException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var freelancerDto = new FreelancerDto { Id = userId.ToString(), StripeAccountId = "acct_123" };
        var stripeException = new StripeException("Stripe error");
        _freelancersGrpcClientMock.Setup(c => c.GetFreelancerByIdAsync(userId.ToString(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(freelancerDto);
        _accountServiceMock.Setup(s => s.GetAsync("acct_123", null, It.IsAny<RequestOptions>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(stripeException);

        // Act
        Func<Task> act = async () => await _service.GetFreelancerAccountAsync(userId, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<BadRequestException>().WithMessage("Stripe error: Stripe error");
        _loggerMock.VerifyLog(LogLevel.Error, $"Stripe error while getting account {freelancerDto.StripeAccountId}: {stripeException.Message}", Times.Once());
    }

    [Fact]
    public async Task GetAllFreelancerAccountsAsync_ValidAccounts_ReturnsAccountList()
    {
        // Arrange
        var accounts = new StripeList<Account>
        {
            Data = new List<Account>
            {
                new Account { Id = "acct_123", Email = "freelancer1@example.com", Type = "custom", Country = "LT" },
                new Account { Id = "acct_456", Email = "freelancer2@example.com", Type = "custom", Country = "LT" }
            }
        };
        var balance1 = new Balance
        {
            Available = new List<BalanceAmount> { new BalanceAmount { Amount = 1000, Currency = "eur" } }
        };
        var balance2 = new Balance
        {
            Available = new List<BalanceAmount> { new BalanceAmount { Amount = 2000, Currency = "eur" } }
        };
        _accountServiceMock.Setup(s => s.ListAsync(It.Is<AccountListOptions>(o => o.Limit == 100), null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(accounts);
        _balanceServiceMock.Setup(s => s.GetAsync(It.IsAny<BalanceGetOptions>(), It.Is<RequestOptions>(o => o.StripeAccount == "acct_123"), It.IsAny<CancellationToken>()))
            .ReturnsAsync(balance1);
        _balanceServiceMock.Setup(s => s.GetAsync(It.IsAny<BalanceGetOptions>(), It.Is<RequestOptions>(o => o.StripeAccount == "acct_456"), It.IsAny<CancellationToken>()))
            .ReturnsAsync(balance2);

        // Act
        var result = await _service.GetAllFreelancerAccountsAsync(CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(new List<FreelancerAccountModel>
        {
            new FreelancerAccountModel { Id = "acct_123", OwnerEmail = "freelancer1@example.com", AccountType = "custom", Country = "LT", Balance = 1000 },
            new FreelancerAccountModel { Id = "acct_456", OwnerEmail = "freelancer2@example.com", AccountType = "custom", Country = "LT", Balance = 2000 }
        });
        _accountServiceMock.Verify(s => s.ListAsync(It.IsAny<AccountListOptions>(), null, It.IsAny<CancellationToken>()), Times.Once());
        _balanceServiceMock.Verify(s => s.GetAsync(It.IsAny<BalanceGetOptions>(), It.IsAny<RequestOptions>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
        _loggerMock.VerifyLog(LogLevel.Information, "Retrieving all Stripe freelancer accounts", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, "Successfully retrieved 2 freelancer accounts", Times.Once());
    }

    [Fact]
    public async Task GetAllFreelancerAccountsAsync_EmptyList_ReturnsEmptyList()
    {
        // Arrange
        var accounts = new StripeList<Account> { Data = new List<Account>() };
        _accountServiceMock.Setup(s => s.ListAsync(It.Is<AccountListOptions>(o => o.Limit == 100), null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(accounts);

        // Act
        var result = await _service.GetAllFreelancerAccountsAsync(CancellationToken.None);

        // Assert
        result.Should().BeEmpty();
        _balanceServiceMock.Verify(s => s.GetAsync(It.IsAny<BalanceGetOptions>(), It.IsAny<RequestOptions>(), It.IsAny<CancellationToken>()), Times.Never());
        _loggerMock.VerifyLog(LogLevel.Information, "Successfully retrieved 0 freelancer accounts", Times.Once());
    }

    [Fact]
    public async Task GetAllFreelancerAccountsAsync_StripeException_ThrowsBadRequestException()
    {
        // Arrange
        var stripeException = new StripeException("Stripe error");
        _accountServiceMock.Setup(s => s.ListAsync(It.IsAny<AccountListOptions>(), null, It.IsAny<CancellationToken>()))
            .ThrowsAsync(stripeException);

        // Act
        Func<Task> act = async () => await _service.GetAllFreelancerAccountsAsync(CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<BadRequestException>().WithMessage("Stripe error: Stripe error");
        _loggerMock.VerifyLog(LogLevel.Error, $"Stripe error while retrieving freelancer accounts: {stripeException.Message}", Times.Once());
    }
}
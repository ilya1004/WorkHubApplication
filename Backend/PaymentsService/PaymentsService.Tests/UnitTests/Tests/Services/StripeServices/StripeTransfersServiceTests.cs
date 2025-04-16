using PaymentsService.Application.Exceptions;
using PaymentsService.Domain.Models;
using PaymentsService.Infrastructure.DTOs;
using PaymentsService.Infrastructure.Interfaces;
using PaymentsService.Infrastructure.Services.StripeTransfersServices;
using PaymentsService.Tests.UnitTests.Extensions;
using Stripe;

namespace PaymentsService.Tests.UnitTests.Tests.Services.StripeServices;

public class StripeTransfersServiceTests
{
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IEmployersGrpcClient> _employersGrpcClientMock;
    private readonly Mock<IFreelancersGrpcClient> _freelancersGrpcClientMock;
    private readonly Mock<ILogger<StripeTransfersService>> _loggerMock;
    private readonly Mock<ChargeService> _chargeServiceMock;
    private readonly Mock<TransferService> _transferServiceMock;
    private readonly Mock<PaymentIntentService> _paymentIntentServiceMock;
    private readonly StripeTransfersService _service;

    public StripeTransfersServiceTests()
    {
        _mapperMock = new Mock<IMapper>();
        _employersGrpcClientMock = new Mock<IEmployersGrpcClient>();
        _freelancersGrpcClientMock = new Mock<IFreelancersGrpcClient>();
        _loggerMock = new Mock<ILogger<StripeTransfersService>>();
        _chargeServiceMock = new Mock<ChargeService>();
        _transferServiceMock = new Mock<TransferService>();
        _paymentIntentServiceMock = new Mock<PaymentIntentService>();

        _service = new StripeTransfersService(
            _mapperMock.Object,
            _employersGrpcClientMock.Object,
            _freelancersGrpcClientMock.Object,
            _loggerMock.Object);

        var chargeServiceField = typeof(StripeTransfersService)
            .GetField("_chargeService", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var transferServiceField = typeof(StripeTransfersService)
            .GetField("_transferService", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var paymentIntentServiceField = typeof(StripeTransfersService)
            .GetField("_paymentIntentService", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        chargeServiceField!.SetValue(_service, _chargeServiceMock.Object);
        transferServiceField!.SetValue(_service, _transferServiceMock.Object);
        paymentIntentServiceField!.SetValue(_service, _paymentIntentServiceMock.Object);
    }

    [Fact]
    public async Task TransferFundsToFreelancer_ValidInput_CreatesTransfer()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var freelancerStripeAccountId = "acct_123";
        var paymentIntent = new PaymentIntentModel { Id = "pi_123", Amount = 10000, Currency = "eur" };
        var transfer = new Transfer { Id = "tr_123" };
        _transferServiceMock.Setup(s => s.CreateAsync(It.Is<TransferCreateOptions>(o =>
            o.Amount == 10000 &&
            o.Currency == "eur" &&
            o.Destination == freelancerStripeAccountId &&
            o.TransferGroup == projectId.ToString() &&
            o.Metadata["project_id"] == projectId.ToString()), null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(transfer);

        // Act
        await _service.TransferFundsToFreelancer(paymentIntent, projectId, freelancerStripeAccountId, CancellationToken.None);

        // Assert
        _transferServiceMock.Verify(s => s.CreateAsync(It.IsAny<TransferCreateOptions>(), null, It.IsAny<CancellationToken>()), Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Transferring funds for project {projectId} to freelancer {freelancerStripeAccountId}", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Funds transferred successfully for project {projectId}", Times.Once());
    }

    [Fact]
    public async Task TransferFundsToFreelancer_StripeException_ThrowsBadRequestException()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var freelancerStripeAccountId = "acct_123";
        var paymentIntent = new PaymentIntentModel { Id = "pi_123", Amount = 10000, Currency = "eur" };
        var stripeException = new StripeException("Stripe error");
        _transferServiceMock.Setup(s => s.CreateAsync(It.IsAny<TransferCreateOptions>(), null, It.IsAny<CancellationToken>()))
            .ThrowsAsync(stripeException);

        // Act
        var act = async () => await _service.TransferFundsToFreelancer(paymentIntent, projectId, freelancerStripeAccountId, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<BadRequestException>().WithMessage("Stripe error: Stripe error");
        _loggerMock.VerifyLog(LogLevel.Error, $"Stripe error transferring funds: {stripeException.Message}", Times.Once());
    }

    [Fact]
    public async Task GetEmployerPaymentsAsync_ValidInput_ReturnsCharges()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var employerDto = new EmployerDto { Id = userId.ToString(), EmployerCustomerId = "cus_123" };
        var charges = new StripeList<Charge>
        {
            Data = new List<Charge>
            {
                new Charge { Id = "ch_123", Amount = 10000, Currency = "eur", Status = "succeeded" },
                new Charge { Id = "ch_456", Amount = 20000, Currency = "eur", Status = "pending" }
            }
        };
        var chargeModels = new List<ChargeModel>
        {
            new ChargeModel { Id = "ch_123", Amount = 10000, Currency = "eur", Status = "succeeded" },
            new ChargeModel { Id = "ch_456", Amount = 20000, Currency = "eur", Status = "pending" }
        };
        _employersGrpcClientMock.Setup(c => c.GetEmployerByIdAsync(userId.ToString(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(employerDto);
        _chargeServiceMock.Setup(s => s.ListAsync(It.Is<ChargeListOptions>(o =>
            o.Customer == "cus_123" && o.TransferGroup == projectId.ToString()), null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(charges);
        _mapperMock.Setup(m => m.Map<ChargeModel>(charges.Data[0])).Returns(chargeModels[0]);
        _mapperMock.Setup(m => m.Map<ChargeModel>(charges.Data[1])).Returns(chargeModels[1]);

        // Act
        var result = await _service.GetEmployerPaymentsAsync(userId, projectId, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(chargeModels);
        _employersGrpcClientMock.Verify(c => c.GetEmployerByIdAsync(userId.ToString(), It.IsAny<CancellationToken>()), Times.Once());
        _chargeServiceMock.Verify(s => s.ListAsync(It.IsAny<ChargeListOptions>(), null, It.IsAny<CancellationToken>()), Times.Once());
        _mapperMock.Verify(m => m.Map<ChargeModel>(It.IsAny<Charge>()), Times.Exactly(2));
        _loggerMock.VerifyLog(LogLevel.Information, $"Getting employer payments for user {userId}, project {projectId}", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Retrieved {charges.Data.Count} charges for employer {userId}", Times.Once());
    }

    [Fact]
    public async Task GetEmployerPaymentsAsync_NoProjectId_ReturnsAllCharges()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var employerDto = new EmployerDto { Id = userId.ToString(), EmployerCustomerId = "cus_123" };
        var charges = new StripeList<Charge>
        {
            Data = new List<Charge> { new Charge { Id = "ch_123", Amount = 10000, Currency = "eur" } }
        };
        var chargeModel = new ChargeModel { Id = "ch_123", Amount = 10000, Currency = "eur" };
        _employersGrpcClientMock.Setup(c => c.GetEmployerByIdAsync(userId.ToString(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(employerDto);
        _chargeServiceMock.Setup(s => s.ListAsync(It.Is<ChargeListOptions>(o => o.Customer == "cus_123" && o.TransferGroup == null), null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(charges);
        _mapperMock.Setup(m => m.Map<ChargeModel>(charges.Data[0])).Returns(chargeModel);

        // Act
        var result = await _service.GetEmployerPaymentsAsync(userId, null, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(new[] { chargeModel });
        _chargeServiceMock.Verify(s => s.ListAsync(It.IsAny<ChargeListOptions>(), null, It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetEmployerPaymentsAsync_NoEmployerAccount_ThrowsNotFoundException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var employerDto = new EmployerDto { Id = userId.ToString(), EmployerCustomerId = null };
        _employersGrpcClientMock.Setup(c => c.GetEmployerByIdAsync(userId.ToString(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(employerDto);

        // Act
        var act = async () => await _service.GetEmployerPaymentsAsync(userId, projectId, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>().WithMessage($"Employer account by employer ID '{userId}' not found.");
        _chargeServiceMock.Verify(s => s.ListAsync(It.IsAny<ChargeListOptions>(), null, It.IsAny<CancellationToken>()), Times.Never());
        _loggerMock.VerifyLog(LogLevel.Warning, $"Employer account not found for user {userId}", Times.Once());
    }

    [Fact]
    public async Task GetAllEmployerPaymentsAsync_ValidInput_ReturnsCharges()
    {
        // Arrange
        var charges = new StripeList<Charge>
        {
            Data = new List<Charge>
            {
                new Charge { Id = "ch_123", Amount = 10000, Currency = "eur" }
            }
        };
        var chargeModel = new ChargeModel { Id = "ch_123", Amount = 10000, Currency = "eur" };
        _chargeServiceMock.Setup(s => s.ListAsync(It.Is<ChargeListOptions>(o => o.Limit == 100), null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(charges);
        _mapperMock.Setup(m => m.Map<ChargeModel>(charges.Data[0])).Returns(chargeModel);

        // Act
        var result = await _service.GetAllEmployerPaymentsAsync(CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(new[] { chargeModel });
        _chargeServiceMock.Verify(s => s.ListAsync(It.IsAny<ChargeListOptions>(), null, It.IsAny<CancellationToken>()), Times.Once());
        _mapperMock.Verify(m => m.Map<ChargeModel>(It.IsAny<Charge>()), Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, "Getting all employer payments", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Retrieved {charges.Data.Count} employer charges", Times.Once());
    }

    [Fact]
    public async Task GetAllEmployerPaymentsAsync_EmptyList_ReturnsEmptyList()
    {
        // Arrange
        var charges = new StripeList<Charge> { Data = new List<Charge>() };
        _chargeServiceMock.Setup(s => s.ListAsync(It.Is<ChargeListOptions>(o => o.Limit == 100), null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(charges);

        // Act
        var result = await _service.GetAllEmployerPaymentsAsync(CancellationToken.None);

        // Assert
        result.Should().BeEmpty();
        _mapperMock.Verify(m => m.Map<ChargeModel>(It.IsAny<Charge>()), Times.Never());
        _loggerMock.VerifyLog(LogLevel.Information, $"Retrieved 0 employer charges", Times.Once());
    }
    
    [Fact]
    public async Task GetEmployerPaymentIntentsAsync_ValidInput_ReturnsPaymentIntents()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var employerDto = new EmployerDto { Id = userId.ToString(), EmployerCustomerId = "cus_123" };
        var paymentIntents = new StripeList<PaymentIntent>
        {
            Data = new List<PaymentIntent>
            {
                new PaymentIntent { Id = "pi_123", Amount = 10000, Currency = "eur", Status = "requires_capture", TransferGroup = projectId.ToString() },
                new PaymentIntent { Id = "pi_456", Amount = 20000, Currency = "eur", Status = "succeeded", TransferGroup = projectId.ToString() }
            }
        };
        var paymentIntentModels = new List<PaymentIntentModel>
        {
            new PaymentIntentModel { Id = "pi_123", Amount = 10000, Currency = "eur", Status = "requires_capture", TransferGroup = projectId.ToString() },
            new PaymentIntentModel { Id = "pi_456", Amount = 20000, Currency = "eur", Status = "succeeded", TransferGroup = projectId.ToString() }
        };
        _employersGrpcClientMock.Setup(c => c.GetEmployerByIdAsync(userId.ToString(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(employerDto);
        _paymentIntentServiceMock.Setup(s => s.ListAsync(It.Is<PaymentIntentListOptions>(o => o.Customer == "cus_123"), null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(paymentIntents);
        _mapperMock.Setup(m => m.Map<PaymentIntentModel>(paymentIntents.Data[0])).Returns(paymentIntentModels[0]);
        _mapperMock.Setup(m => m.Map<PaymentIntentModel>(paymentIntents.Data[1])).Returns(paymentIntentModels[1]);

        // Act
        var result = await _service.GetEmployerPaymentIntentsAsync(userId, projectId, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(paymentIntentModels);
        _employersGrpcClientMock.Verify(c => c.GetEmployerByIdAsync(userId.ToString(), It.IsAny<CancellationToken>()), Times.Once());
        _paymentIntentServiceMock.Verify(s => s.ListAsync(It.IsAny<PaymentIntentListOptions>(), null, It.IsAny<CancellationToken>()), Times.Once());
        _mapperMock.Verify(m => m.Map<PaymentIntentModel>(It.IsAny<PaymentIntent>()), Times.Exactly(2));
        _loggerMock.VerifyLog(LogLevel.Information, $"Getting employer payment intents for user {userId}, project {projectId}", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Retrieved {paymentIntents.Data.Count} payment intents for employer {userId}", Times.Once());
    }

    [Fact]
    public async Task GetEmployerPaymentIntentsAsync_NoProjectId_ReturnsAllIntents()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var employerDto = new EmployerDto { Id = userId.ToString(), EmployerCustomerId = "cus_123" };
        var paymentIntents = new StripeList<PaymentIntent>
        {
            Data = new List<PaymentIntent>
            {
                new PaymentIntent { Id = "pi_123", Amount = 10000, Currency = "eur" }
            }
        };
        var paymentIntentModel = new PaymentIntentModel { Id = "pi_123", Amount = 10000, Currency = "eur" };
        _employersGrpcClientMock.Setup(c => c.GetEmployerByIdAsync(userId.ToString(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(employerDto);
        _paymentIntentServiceMock.Setup(s => s.ListAsync(It.IsAny<PaymentIntentListOptions>(), null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(paymentIntents);
        _mapperMock.Setup(m => m.Map<PaymentIntentModel>(paymentIntents.Data[0])).Returns(paymentIntentModel);

        // Act
        var result = await _service.GetEmployerPaymentIntentsAsync(userId, null, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(new[] { paymentIntentModel });
    }

    [Fact]
    public async Task GetEmployerPaymentIntentsAsync_NoEmployerAccount_ThrowsNotFoundException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var employerDto = new EmployerDto { Id = userId.ToString(), EmployerCustomerId = null };
        _employersGrpcClientMock.Setup(c => c.GetEmployerByIdAsync(userId.ToString(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(employerDto);

        // Act
        var act = async () => await _service.GetEmployerPaymentIntentsAsync(userId, projectId, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>().WithMessage($"Employer account by employer ID '{userId}' not found.");
        _paymentIntentServiceMock.Verify(s => s.ListAsync(It.IsAny<PaymentIntentListOptions>(), null, It.IsAny<CancellationToken>()), Times.Never());
        _loggerMock.VerifyLog(LogLevel.Warning, $"Employer account not found for user {userId}", Times.Once());
    }

    [Fact]
    public async Task GetFreelancerTransfersAsync_ValidInput_ReturnsTransfers()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var freelancerDto = new FreelancerDto { Id = userId.ToString(), StripeAccountId = "acct_123" };
        var transfers = new StripeList<Transfer>
        {
            Data = new List<Transfer>
            {
                new Transfer { Id = "tr_123", Amount = 10000, Currency = "eur" },
                new Transfer { Id = "tr_456", Amount = 20000, Currency = "eur" }
            }
        };
        var transferModels = new List<TransferModel>
        {
            new TransferModel { Id = "tr_123", Amount = 10000, Currency = "eur" },
            new TransferModel { Id = "tr_456", Amount = 20000, Currency = "eur" }
        };
        _freelancersGrpcClientMock.Setup(c => c.GetFreelancerByIdAsync(userId.ToString(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(freelancerDto);
        _transferServiceMock.Setup(s => s.ListAsync(It.Is<TransferListOptions>(o =>
            o.Destination == "acct_123" && o.TransferGroup == projectId.ToString()), null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(transfers);
        _mapperMock.Setup(m => m.Map<TransferModel>(transfers.Data[0])).Returns(transferModels[0]);
        _mapperMock.Setup(m => m.Map<TransferModel>(transfers.Data[1])).Returns(transferModels[1]);

        // Act
        var result = await _service.GetFreelancerTransfersAsync(userId, projectId, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(transferModels);
        _freelancersGrpcClientMock.Verify(c => c.GetFreelancerByIdAsync(userId.ToString(), It.IsAny<CancellationToken>()), Times.Once());
        _transferServiceMock.Verify(s => s.ListAsync(It.IsAny<TransferListOptions>(), null, It.IsAny<CancellationToken>()), Times.Once());
        _mapperMock.Verify(m => m.Map<TransferModel>(It.IsAny<Transfer>()), Times.Exactly(2));
        _loggerMock.VerifyLog(LogLevel.Information, $"Getting freelancer transfers for user {userId}, project {projectId}", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Retrieved {transfers.Data.Count} transfers for freelancer {userId}", Times.Once());
    }

    [Fact]
    public async Task GetFreelancerTransfersAsync_NoProjectId_ReturnsAllTransfers()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var freelancerDto = new FreelancerDto { Id = userId.ToString(), StripeAccountId = "acct_123" };
        var transfers = new StripeList<Transfer>
        {
            Data = new List<Transfer> { new Transfer { Id = "tr_123", Amount = 10000, Currency = "eur" } }
        };
        var transferModel = new TransferModel { Id = "tr_123", Amount = 10000, Currency = "eur" };
        _freelancersGrpcClientMock.Setup(c => c.GetFreelancerByIdAsync(userId.ToString(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(freelancerDto);
        _transferServiceMock.Setup(s => s.ListAsync(It.Is<TransferListOptions>(o => o.Destination == "acct_123" && o.TransferGroup == null), null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(transfers);
        _mapperMock.Setup(m => m.Map<TransferModel>(transfers.Data[0])).Returns(transferModel);

        // Act
        var result = await _service.GetFreelancerTransfersAsync(userId, null, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(new[] { transferModel });
    }

    [Fact]
    public async Task GetFreelancerTransfersAsync_NoFreelancerAccount_ThrowsNotFoundException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var freelancerDto = new FreelancerDto { Id = userId.ToString(), StripeAccountId = null };
        _freelancersGrpcClientMock.Setup(c => c.GetFreelancerByIdAsync(userId.ToString(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(freelancerDto);

        // Act
        var act = async () => await _service.GetFreelancerTransfersAsync(userId, projectId, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>().WithMessage($"Stripe account with user ID '{userId}' not found.");
        _transferServiceMock.Verify(s => s.ListAsync(It.IsAny<TransferListOptions>(), null, It.IsAny<CancellationToken>()), Times.Never());
        _loggerMock.VerifyLog(LogLevel.Warning, $"Freelancer account not found for user {userId}", Times.Once());
    }

    [Fact]
    public async Task GetAllFreelancerTransfersAsync_ValidInput_ReturnsTransfers()
    {
        // Arrange
        var transfers = new StripeList<Transfer>
        {
            Data = new List<Transfer>
            {
                new Transfer { Id = "tr_123", Amount = 10000, Currency = "eur" }
            }
        };
        var transferModel = new TransferModel { Id = "tr_123", Amount = 10000, Currency = "eur" };
        _transferServiceMock.Setup(s => s.ListAsync(It.Is<TransferListOptions>(o => o.Limit == 100), null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(transfers);
        _mapperMock.Setup(m => m.Map<TransferModel>(transfers.Data[0])).Returns(transferModel);

        // Act
        var result = await _service.GetAllFreelancerTransfersAsync(CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(new[] { transferModel });
        _transferServiceMock.Verify(s => s.ListAsync(It.IsAny<TransferListOptions>(), null, It.IsAny<CancellationToken>()), Times.Once());
        _mapperMock.Verify(m => m.Map<TransferModel>(It.IsAny<Transfer>()), Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, "Getting all freelancer transfers", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Retrieved {transfers.Data.Count} freelancer transfers", Times.Once());
    }

    [Fact]
    public async Task GetAllFreelancerTransfersAsync_EmptyList_ReturnsEmptyList()
    {
        // Arrange
        var transfers = new StripeList<Transfer> { Data = new List<Transfer>() };
        _transferServiceMock.Setup(s => s.ListAsync(It.Is<TransferListOptions>(o => o.Limit == 100), null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(transfers);

        // Act
        var result = await _service.GetAllFreelancerTransfersAsync(CancellationToken.None);

        // Assert
        result.Should().BeEmpty();
        _mapperMock.Verify(m => m.Map<TransferModel>(It.IsAny<Transfer>()), Times.Never());
        _loggerMock.VerifyLog(LogLevel.Information, $"Retrieved 0 freelancer transfers", Times.Once());
    }
}
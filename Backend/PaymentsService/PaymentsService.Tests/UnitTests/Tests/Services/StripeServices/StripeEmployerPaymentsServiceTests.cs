using PaymentsService.Application.Exceptions;
using PaymentsService.Domain.Abstractions.KafkaProducerServices;
using PaymentsService.Domain.Abstractions.TransfersServices;
using PaymentsService.Domain.Models;
using PaymentsService.Infrastructure.DTOs;
using PaymentsService.Infrastructure.Interfaces;
using PaymentsService.Infrastructure.Services.StripePaymentsServices;
using PaymentsService.Tests.UnitTests.Extensions;
using Stripe;

namespace PaymentsService.Tests.UnitTests.Tests.Services.StripeServices;

public class StripeEmployerPaymentsServiceTests
{
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ITransfersService> _transfersServiceMock;
    private readonly Mock<IPaymentsProducerService> _paymentsProducerServiceMock;
    private readonly Mock<IEmployersGrpcClient> _employersGrpcClientMock;
    private readonly Mock<IProjectsGrpcClient> _projectsGrpcClientMock;
    private readonly Mock<IFreelancersGrpcClient> _freelancersGrpcClientMock;
    private readonly Mock<ILogger<StripeEmployerPaymentsService>> _loggerMock;
    private readonly Mock<CustomerPaymentMethodService> _customerPaymentMethodServiceMock;
    private readonly Mock<PaymentIntentService> _paymentIntentServiceMock;
    private readonly Mock<AccountService> _accountServiceMock;
    private readonly StripeEmployerPaymentsService _service;

    public StripeEmployerPaymentsServiceTests()
    {
        _mapperMock = new Mock<IMapper>();
        _transfersServiceMock = new Mock<ITransfersService>();
        _paymentsProducerServiceMock = new Mock<IPaymentsProducerService>();
        _employersGrpcClientMock = new Mock<IEmployersGrpcClient>();
        _projectsGrpcClientMock = new Mock<IProjectsGrpcClient>();
        _freelancersGrpcClientMock = new Mock<IFreelancersGrpcClient>();
        _loggerMock = new Mock<ILogger<StripeEmployerPaymentsService>>();
        _customerPaymentMethodServiceMock = new Mock<CustomerPaymentMethodService>();
        _paymentIntentServiceMock = new Mock<PaymentIntentService>();
        _accountServiceMock = new Mock<AccountService>();

        _service = new StripeEmployerPaymentsService(
            _mapperMock.Object,
            _transfersServiceMock.Object,
            _paymentsProducerServiceMock.Object,
            _employersGrpcClientMock.Object,
            _projectsGrpcClientMock.Object,
            _freelancersGrpcClientMock.Object,
            _loggerMock.Object);

        var customerPaymentMethodField = typeof(StripeEmployerPaymentsService)
            .GetField("_customerPaymentMethodService", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var paymentIntentField = typeof(StripeEmployerPaymentsService)
            .GetField("_paymentIntentService", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var accountField = typeof(StripeEmployerPaymentsService)
            .GetField("_accountService", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        customerPaymentMethodField!.SetValue(_service, _customerPaymentMethodServiceMock.Object);
        paymentIntentField!.SetValue(_service, _paymentIntentServiceMock.Object);
        accountField!.SetValue(_service, _accountServiceMock.Object);
    }

    [Fact]
    public async Task CreatePaymentIntentWithSavedMethodAsync_ValidInput_CreatesPaymentIntent()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var paymentMethodId = "pm_123";
        var employerDto = new EmployerDto { Id = userId.ToString(), EmployerCustomerId = "cus_123" };
        var projectDto = new ProjectDto { Id = projectId, BudgetInCents = 10000 };
        var paymentMethod = new PaymentMethod { Id = paymentMethodId };
        var paymentIntent = new PaymentIntent { Id = "pi_123" };
        _employersGrpcClientMock.Setup(c => c.GetEmployerByIdAsync(userId.ToString(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(employerDto);
        _projectsGrpcClientMock.Setup(c => c.GetProjectByIdAsync(projectId.ToString(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(projectDto);
        _customerPaymentMethodServiceMock.Setup(s => s.GetAsync("cus_123", paymentMethodId, null, It.IsAny<RequestOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(paymentMethod);
        _paymentIntentServiceMock.Setup(s => s.CreateAsync(It.Is<PaymentIntentCreateOptions>(o =>
            o.Amount == 10000 &&
            o.Currency == "eur" &&
            o.Customer == "cus_123" &&
            o.PaymentMethod == paymentMethodId &&
            o.Confirm == true &&
            o.CaptureMethod == "manual" &&
            o.TransferGroup == projectId.ToString()), null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(paymentIntent);
        _paymentsProducerServiceMock.Setup(s => s.SavePaymentIntentIdAsync(projectId.ToString(), "pi_123", It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _service.CreatePaymentIntentWithSavedMethodAsync(userId, projectId, paymentMethodId, CancellationToken.None);

        // Assert
        _employersGrpcClientMock.Verify(c => c.GetEmployerByIdAsync(userId.ToString(), It.IsAny<CancellationToken>()), Times.Once());
        _projectsGrpcClientMock.Verify(c => c.GetProjectByIdAsync(projectId.ToString(), It.IsAny<CancellationToken>()), Times.Once());
        _customerPaymentMethodServiceMock.Verify(s => s.GetAsync("cus_123", paymentMethodId, null, It.IsAny<RequestOptions>(), It.IsAny<CancellationToken>()), Times.Once());
        _paymentIntentServiceMock.Verify(s => s.CreateAsync(It.IsAny<PaymentIntentCreateOptions>(), null, It.IsAny<CancellationToken>()), Times.Once());
        _paymentsProducerServiceMock.Verify(s => s.SavePaymentIntentIdAsync(projectId.ToString(), "pi_123", It.IsAny<CancellationToken>()), Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Creating payment intent for project {projectId} with method {paymentMethodId} by user {userId}", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Retrieving payment method {paymentMethodId}", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Payment intent {paymentIntent.Id} created successfully for project {projectId}", Times.Once());
    }

    [Fact]
    public async Task CreatePaymentIntentWithSavedMethodAsync_NoEmployerAccount_ThrowsNotFoundException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var paymentMethodId = "pm_123";
        var employerDto = new EmployerDto { Id = userId.ToString(), EmployerCustomerId = null };
        _employersGrpcClientMock.Setup(c => c.GetEmployerByIdAsync(userId.ToString(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(employerDto);

        // Act
        var act = async () => await _service.CreatePaymentIntentWithSavedMethodAsync(userId, projectId, paymentMethodId, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>().WithMessage($"Employer account by employer ID '{userId}' not found.");
        _projectsGrpcClientMock.Verify(c => c.GetProjectByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never());
        _customerPaymentMethodServiceMock.Verify(s => s.GetAsync(It.IsAny<string>(), It.IsAny<string>(), null, It.IsAny<RequestOptions>(), It.IsAny<CancellationToken>()), Times.Never());
        _loggerMock.VerifyLog(LogLevel.Warning, $"Employer account not found for user {userId}", Times.Once());
    }

    [Fact]
    public async Task CreatePaymentIntentWithSavedMethodAsync_PaymentMethodNotFound_ThrowsBadRequestException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var paymentMethodId = "pm_123";
        var employerDto = new EmployerDto { Id = userId.ToString(), EmployerCustomerId = "cus_123" };
        var projectDto = new ProjectDto { Id = projectId, BudgetInCents = 10000 };
        _employersGrpcClientMock.Setup(c => c.GetEmployerByIdAsync(userId.ToString(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(employerDto);
        _projectsGrpcClientMock.Setup(c => c.GetProjectByIdAsync(projectId.ToString(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(projectDto);
        _customerPaymentMethodServiceMock.Setup(s => s.GetAsync("cus_123", paymentMethodId, null, It.IsAny<RequestOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((PaymentMethod?)null);

        // Act
        var act = async () => await _service.CreatePaymentIntentWithSavedMethodAsync(userId, projectId, paymentMethodId, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<BadRequestException>().WithMessage($"Could not create Payment intent for project with ID '{projectId}'.");
        _paymentIntentServiceMock.Verify(s => s.CreateAsync(It.IsAny<PaymentIntentCreateOptions>(), null, It.IsAny<CancellationToken>()), Times.Never());
        _loggerMock.VerifyLog(LogLevel.Error, $"Payment method {paymentMethodId} not found", Times.Once());
    }

    [Fact]
    public async Task CreatePaymentIntentWithSavedMethodAsync_StripeException_ThrowsBadRequestException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var paymentMethodId = "pm_123";
        var employerDto = new EmployerDto { Id = userId.ToString(), EmployerCustomerId = "cus_123" };
        var projectDto = new ProjectDto { Id = projectId, BudgetInCents = 10000 };
        var paymentMethod = new PaymentMethod { Id = paymentMethodId };
        var stripeException = new StripeException("Stripe error");
        _employersGrpcClientMock.Setup(c => c.GetEmployerByIdAsync(userId.ToString(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(employerDto);
        _projectsGrpcClientMock.Setup(c => c.GetProjectByIdAsync(projectId.ToString(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(projectDto);
        _customerPaymentMethodServiceMock.Setup(s => s.GetAsync("cus_123", paymentMethodId, null, It.IsAny<RequestOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(paymentMethod);
        _paymentIntentServiceMock.Setup(s => s.CreateAsync(It.IsAny<PaymentIntentCreateOptions>(), null, It.IsAny<CancellationToken>()))
            .ThrowsAsync(stripeException);

        // Act
        var act = async () => await _service.CreatePaymentIntentWithSavedMethodAsync(userId, projectId, paymentMethodId, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<BadRequestException>().WithMessage("Stripe error: Stripe error");
        _loggerMock.VerifyLog(LogLevel.Error, $"Stripe error creating payment intent: {stripeException.Message}", Times.Once());
    }

    [Fact]
    public async Task ConfirmPaymentForProjectAsync_ValidInput_ConfirmsPaymentAndTransfersFunds()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var freelancerId = Guid.NewGuid();
        var projectDto = new ProjectDto { Id = projectId, PaymentIntentId = "pi_123", FreelancerId = freelancerId };
        var freelancerDto = new FreelancerDto { Id = freelancerId.ToString(), StripeAccountId = "acct_123" };
        var account = new Account { Id = "acct_123", ChargesEnabled = true, PayoutsEnabled = true };
        var paymentIntent = new PaymentIntent { Id = "pi_123", Status = "requires_capture", Amount = 10000 };
        var capturedPaymentIntent = new PaymentIntent { Id = "pi_123", Status = "succeeded", Amount = 10000 };
        var paymentIntentModel = new PaymentIntentModel { Id = "pi_123" };
        _projectsGrpcClientMock.Setup(c => c.GetProjectByIdAsync(projectId.ToString(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(projectDto);
        _freelancersGrpcClientMock.Setup(c => c.GetFreelancerByIdAsync(freelancerId.ToString(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(freelancerDto);
        _accountServiceMock.Setup(s => s.GetAsync("acct_123", null, It.IsAny<RequestOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(account);
        _paymentIntentServiceMock.Setup(s => s.GetAsync("pi_123", null, It.IsAny<RequestOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(paymentIntent);
        _paymentIntentServiceMock.Setup(s => s.CaptureAsync("pi_123", It.Is<PaymentIntentCaptureOptions>(o => o.AmountToCapture == 10000), null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(capturedPaymentIntent);
        _mapperMock.Setup(m => m.Map<PaymentIntentModel>(capturedPaymentIntent))
            .Returns(paymentIntentModel);
        _transfersServiceMock.Setup(s => s.TransferFundsToFreelancer(paymentIntentModel, projectId, "acct_123", It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _service.ConfirmPaymentForProjectAsync(projectId, CancellationToken.None);

        // Assert
        _projectsGrpcClientMock.Verify(c => c.GetProjectByIdAsync(projectId.ToString(), It.IsAny<CancellationToken>()), Times.Once());
        _freelancersGrpcClientMock.Verify(c => c.GetFreelancerByIdAsync(freelancerId.ToString(), It.IsAny<CancellationToken>()), Times.Once());
        _accountServiceMock.Verify(s => s.GetAsync("acct_123", null, It.IsAny<RequestOptions>(), It.IsAny<CancellationToken>()), Times.Once());
        _paymentIntentServiceMock.Verify(s => s.GetAsync("pi_123", null, It.IsAny<RequestOptions>(), It.IsAny<CancellationToken>()), Times.Once());
        _paymentIntentServiceMock.Verify(s => s.CaptureAsync("pi_123", It.IsAny<PaymentIntentCaptureOptions>(), null, It.IsAny<CancellationToken>()), Times.Once());
        _mapperMock.Verify(m => m.Map<PaymentIntentModel>(capturedPaymentIntent), Times.Once());
        _transfersServiceMock.Verify(s => s.TransferFundsToFreelancer(paymentIntentModel, projectId, "acct_123", It.IsAny<CancellationToken>()), Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Confirming payment for project {projectId}", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Retrieving payment intent {projectDto.PaymentIntentId}", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Capturing payment intent {paymentIntent.Id}", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Payment intent {paymentIntent.Id} captured successfully", Times.Once());
    }

    [Fact]
    public async Task ConfirmPaymentForProjectAsync_NoPaymentIntent_ThrowsNotFoundException()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var projectDto = new ProjectDto { Id = projectId, PaymentIntentId = null };
        _projectsGrpcClientMock.Setup(c => c.GetProjectByIdAsync(projectId.ToString(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(projectDto);

        // Act
        var act = async () => await _service.ConfirmPaymentForProjectAsync(projectId, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>().WithMessage("This project does not have an attached Payment Intent.");
        _freelancersGrpcClientMock.Verify(c => c.GetFreelancerByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never());
        _loggerMock.VerifyLog(LogLevel.Warning, $"Payment intent not found for project {projectId}", Times.Once());
    }

    [Fact]
    public async Task ConfirmPaymentForProjectAsync_NoFreelancer_ThrowsNotFoundException()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var projectDto = new ProjectDto { Id = projectId, PaymentIntentId = "pi_123", FreelancerId = null };
        _projectsGrpcClientMock.Setup(c => c.GetProjectByIdAsync(projectId.ToString(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(projectDto);

        // Act
        var act = async () => await _service.ConfirmPaymentForProjectAsync(projectId, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>().WithMessage("This project does not have freelancer.");
        _freelancersGrpcClientMock.Verify(c => c.GetFreelancerByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never());
        _loggerMock.VerifyLog(LogLevel.Warning, $"Freelancer user not found for project {projectId}", Times.Once());
    }

    [Fact]
    public async Task ConfirmPaymentForProjectAsync_NoFreelancerAccount_ThrowsNotFoundException()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var freelancerId = Guid.NewGuid();
        var projectDto = new ProjectDto { Id = projectId, PaymentIntentId = "pi_123", FreelancerId = freelancerId };
        var freelancerDto = new FreelancerDto { Id = freelancerId.ToString(), StripeAccountId = null };
        _projectsGrpcClientMock.Setup(c => c.GetProjectByIdAsync(projectId.ToString(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(projectDto);
        _freelancersGrpcClientMock.Setup(c => c.GetFreelancerByIdAsync(freelancerId.ToString(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(freelancerDto);

        // Act
        var act = async () => await _service.ConfirmPaymentForProjectAsync(projectId, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>().WithMessage($"Freelancer's Stripe Account to project with ID '{projectId}' not found.");
        _accountServiceMock.Verify(s => s.GetAsync(It.IsAny<string>(), null, It.IsAny<RequestOptions>(), It.IsAny<CancellationToken>()), Times.Never());
        _loggerMock.VerifyLog(LogLevel.Warning, $"Freelancer account not found for project {projectId}", Times.Once());
    }

    [Fact]
    public async Task ConfirmPaymentForProjectAsync_StripeException_ThrowsBadRequestException()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var freelancerId = Guid.NewGuid();
        var projectDto = new ProjectDto { Id = projectId, PaymentIntentId = "pi_123", FreelancerId = freelancerId };
        var freelancerDto = new FreelancerDto { Id = freelancerId.ToString(), StripeAccountId = "acct_123" };
        var account = new Account { Id = "acct_123", ChargesEnabled = true, PayoutsEnabled = true };
        var paymentIntent = new PaymentIntent { Id = "pi_123", Status = "requires_capture", Amount = 10000 };
        var stripeException = new StripeException("Stripe error");
        _projectsGrpcClientMock.Setup(c => c.GetProjectByIdAsync(projectId.ToString(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(projectDto);
        _freelancersGrpcClientMock.Setup(c => c.GetFreelancerByIdAsync(freelancerId.ToString(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(freelancerDto);
        _accountServiceMock.Setup(s => s.GetAsync("acct_123", null, It.IsAny<RequestOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(account);
        _paymentIntentServiceMock.Setup(s => s.GetAsync("pi_123", null, It.IsAny<RequestOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(paymentIntent);
        _paymentIntentServiceMock.Setup(s => s.CaptureAsync("pi_123", It.IsAny<PaymentIntentCaptureOptions>(), null, It.IsAny<CancellationToken>()))
            .ThrowsAsync(stripeException);

        // Act
        var act = async () => await _service.ConfirmPaymentForProjectAsync(projectId, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<BadRequestException>().WithMessage("Stripe error: Stripe error");
        _loggerMock.VerifyLog(LogLevel.Error, $"Stripe error confirming payment: {stripeException.Message}", Times.Once());
    }

    [Fact]
    public async Task CancelPaymentIntentForProjectAsync_ValidInput_CancelsPaymentIntent()
    {
        // Arrange
        var paymentIntentId = "pi_123";
        var paymentIntent = new PaymentIntent { Id = paymentIntentId, Status = "requires_capture" };
        _paymentIntentServiceMock.Setup(s => s.GetAsync(paymentIntentId, null, It.IsAny<RequestOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(paymentIntent);
        _paymentIntentServiceMock.Setup(s => s.CancelAsync(paymentIntentId, null, It.IsAny<RequestOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(paymentIntent);

        // Act
        await _service.CancelPaymentIntentForProjectAsync(paymentIntentId, CancellationToken.None);

        // Assert
        _paymentIntentServiceMock.Verify(s => s.GetAsync(paymentIntentId, null, It.IsAny<RequestOptions>(), It.IsAny<CancellationToken>()), Times.Once());
        _paymentIntentServiceMock.Verify(s => s.CancelAsync(paymentIntentId, null, It.IsAny<RequestOptions>(), It.IsAny<CancellationToken>()), Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Canceling payment intent {paymentIntentId}", Times.Exactly(2));
        _loggerMock.VerifyLog(LogLevel.Information, $"Retrieving payment intent {paymentIntentId}", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Payment intent {paymentIntentId} canceled successfully", Times.Once());
    }

    [Fact]
    public async Task CancelPaymentIntentForProjectAsync_EmptyPaymentIntentId_ThrowsNotFoundException()
    {
        // Arrange
        var paymentIntentId = "";

        // Act
        var act = async () => await _service.CancelPaymentIntentForProjectAsync(paymentIntentId, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>().WithMessage("This project does not have an attached Payment Intent.");
        _paymentIntentServiceMock.Verify(s => s.GetAsync(It.IsAny<string>(), null, It.IsAny<RequestOptions>(), It.IsAny<CancellationToken>()), Times.Never());
        _loggerMock.VerifyLog(LogLevel.Warning, "Payment intent ID not provided", Times.Once());
    }

    [Fact]
    public async Task CancelPaymentIntentForProjectAsync_PaymentIntentNotFound_ThrowsNotFoundException()
    {
        // Arrange
        var paymentIntentId = "pi_123";
        _paymentIntentServiceMock.Setup(s => s.GetAsync(paymentIntentId, null, It.IsAny<RequestOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((PaymentIntent?)null);

        // Act
        var act = async () => await _service.CancelPaymentIntentForProjectAsync(paymentIntentId, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<BadRequestException>().WithMessage($"Could not cancel Payment Intent with ID '{paymentIntentId}'.");
        _paymentIntentServiceMock.Verify(s => s.CancelAsync(It.IsAny<string>(), null, It.IsAny<RequestOptions>(), It.IsAny<CancellationToken>()), Times.Never());
        _loggerMock.VerifyLog(LogLevel.Error, $"Payment intent {paymentIntentId} not found", Times.Once());
    }
    
    [Fact]
    public async Task CancelPaymentIntentForProjectAsync_StripeException_ThrowsBadRequestException()
    {
        // Arrange
        var paymentIntentId = "pi_123";
        var paymentIntent = new PaymentIntent { Id = paymentIntentId, Status = "requires_capture" };
        var stripeException = new StripeException("Stripe error");
        _paymentIntentServiceMock.Setup(s => s.GetAsync(paymentIntentId, null, It.IsAny<RequestOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(paymentIntent);
        _paymentIntentServiceMock.Setup(s => s.CancelAsync(paymentIntentId, null, It.IsAny<RequestOptions>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(stripeException);

        // Act
        var act = async () => await _service.CancelPaymentIntentForProjectAsync(paymentIntentId, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<BadRequestException>().WithMessage("Stripe error: Stripe error");
        _loggerMock.VerifyLog(LogLevel.Error, $"Stripe error canceling payment intent: {stripeException.Message}", Times.Once());
    }
}
using PaymentsService.Application.Exceptions;
using PaymentsService.Domain.Models;
using PaymentsService.Infrastructure.DTOs;
using PaymentsService.Infrastructure.Interfaces;
using PaymentsService.Infrastructure.Services.StripePaymentsServices;
using PaymentsService.Tests.UnitTests.Extensions;
using Stripe;

namespace PaymentsService.Tests.UnitTests.Tests.Services.StripeServices;

public class StripePaymentMethodsServiceTests
{
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IEmployersGrpcClient> _employersGrpcClientMock;
    private readonly Mock<ILogger<StripePaymentMethodsService>> _loggerMock;
    private readonly Mock<CustomerPaymentMethodService> _customerPaymentMethodServiceMock;
    private readonly Mock<PaymentMethodService> _paymentMethodServiceMock;
    private readonly StripePaymentMethodsService _service;

    public StripePaymentMethodsServiceTests()
    {
        _mapperMock = new Mock<IMapper>();
        _employersGrpcClientMock = new Mock<IEmployersGrpcClient>();
        _loggerMock = new Mock<ILogger<StripePaymentMethodsService>>();
        _customerPaymentMethodServiceMock = new Mock<CustomerPaymentMethodService>();
        _paymentMethodServiceMock = new Mock<PaymentMethodService>();

        _service = new StripePaymentMethodsService(
            _mapperMock.Object,
            _employersGrpcClientMock.Object,
            _loggerMock.Object);
        
        var customerPaymentMethodField = typeof(StripePaymentMethodsService)
            .GetField("_customerPaymentMethodService", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var paymentMethodField = typeof(StripePaymentMethodsService)
            .GetField("_paymentMethodService", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        customerPaymentMethodField!.SetValue(_service, _customerPaymentMethodServiceMock.Object);
        paymentMethodField!.SetValue(_service, _paymentMethodServiceMock.Object);
    }

    [Fact]
    public async Task SavePaymentMethodAsync_ValidInput_SavesPaymentMethod()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var paymentMethodId = "pm_123";
        var employerDto = new EmployerDto { Id = userId.ToString(), EmployerCustomerId = "cus_123" };
        var paymentMethod = new PaymentMethod { Id = paymentMethodId };
        _employersGrpcClientMock.Setup(c => c.GetEmployerByIdAsync(userId.ToString(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(employerDto);
        _paymentMethodServiceMock.Setup(s => s.GetAsync(paymentMethodId, null, It.IsAny<RequestOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(paymentMethod);
        _paymentMethodServiceMock.Setup(s => s.AttachAsync(paymentMethodId, It.Is<PaymentMethodAttachOptions>(o => o.Customer == "cus_123"), null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(paymentMethod);

        // Act
        await _service.SavePaymentMethodAsync(userId, paymentMethodId, CancellationToken.None);

        // Assert
        _employersGrpcClientMock.Verify(c => c.GetEmployerByIdAsync(userId.ToString(), It.IsAny<CancellationToken>()), Times.Once());
        _paymentMethodServiceMock.Verify(s => s.GetAsync(paymentMethodId, null, It.IsAny<RequestOptions>(), It.IsAny<CancellationToken>()), Times.Once());
        _paymentMethodServiceMock.Verify(s => s.AttachAsync(paymentMethodId, It.IsAny<PaymentMethodAttachOptions>(), null, It.IsAny<CancellationToken>()), Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Saving payment method {paymentMethodId} for user {userId}", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Retrieving payment method {paymentMethodId}", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Attaching payment method {paymentMethodId} to customer {employerDto.EmployerCustomerId}", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Payment method {paymentMethodId} saved successfully for user {userId}", Times.Once());
    }

    [Fact]
    public async Task SavePaymentMethodAsync_NoEmployerAccount_ThrowsNotFoundException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var paymentMethodId = "pm_123";
        var employerDto = new EmployerDto { Id = userId.ToString(), EmployerCustomerId = null };
        _employersGrpcClientMock.Setup(c => c.GetEmployerByIdAsync(userId.ToString(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(employerDto);

        // Act
        var act = async () => await _service.SavePaymentMethodAsync(userId, paymentMethodId, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>().WithMessage($"Employer account by employer ID '{userId}' not found.");
        _paymentMethodServiceMock.Verify(s => s.GetAsync(It.IsAny<string>(), null, It.IsAny<RequestOptions>(), It.IsAny<CancellationToken>()), Times.Never());
        _loggerMock.VerifyLog(LogLevel.Warning, $"Employer account not found for user {userId}", Times.Once());
    }

    [Fact]
    public async Task SavePaymentMethodAsync_PaymentMethodNotFound_ThrowsNotFoundException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var paymentMethodId = "pm_123";
        var employerDto = new EmployerDto { Id = userId.ToString(), EmployerCustomerId = "cus_123" };
        _employersGrpcClientMock.Setup(c => c.GetEmployerByIdAsync(userId.ToString(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(employerDto);
        _paymentMethodServiceMock.Setup(s => s.GetAsync(paymentMethodId, null, It.IsAny<RequestOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((PaymentMethod?)null);

        // Act
        var act = async () => await _service.SavePaymentMethodAsync(userId, paymentMethodId, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<BadRequestException>().WithMessage($"Could not save your Payment method for employer with ID '{employerDto.Id}'.");
        _paymentMethodServiceMock.Verify(s => s.AttachAsync(It.IsAny<string>(), It.IsAny<PaymentMethodAttachOptions>(), null, It.IsAny<CancellationToken>()), Times.Never());
        _loggerMock.VerifyLog(LogLevel.Error, $"Payment method {paymentMethodId} not found", Times.Once());
    }

    [Fact]
    public async Task SavePaymentMethodAsync_StripeException_ThrowsBadRequestException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var paymentMethodId = "pm_123";
        var employerDto = new EmployerDto { Id = userId.ToString(), EmployerCustomerId = "cus_123" };
        var paymentMethod = new PaymentMethod { Id = paymentMethodId };
        var stripeException = new StripeException("Stripe error");
        _employersGrpcClientMock.Setup(c => c.GetEmployerByIdAsync(userId.ToString(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(employerDto);
        _paymentMethodServiceMock.Setup(s => s.GetAsync(paymentMethodId, null, It.IsAny<RequestOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(paymentMethod);
        _paymentMethodServiceMock.Setup(s => s.AttachAsync(paymentMethodId, It.IsAny<PaymentMethodAttachOptions>(), null, It.IsAny<CancellationToken>()))
            .ThrowsAsync(stripeException);

        // Act
        var act = async () => await _service.SavePaymentMethodAsync(userId, paymentMethodId, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<BadRequestException>().WithMessage("Stripe error: Stripe error");
        _loggerMock.VerifyLog(LogLevel.Error, $"Stripe error saving payment method: {stripeException.Message}", Times.Once());
    }

    [Fact]
    public async Task GetPaymentMethodsAsync_ValidInput_ReturnsPaymentMethods()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var employerDto = new EmployerDto { Id = userId.ToString(), EmployerCustomerId = "cus_123" };
        var paymentMethods = new StripeList<PaymentMethod>
        {
            Data = new List<PaymentMethod>
            {
                new PaymentMethod { Id = "pm_123", Type = "card", Created = DateTime.UtcNow, Card = new PaymentMethodCard { Brand = "visa", Country = "US", ExpMonth = 12, ExpYear = 2025, Last4 = "4242" } },
                new PaymentMethod { Id = "pm_456", Type = "card", Created = DateTime.UtcNow, Card = new PaymentMethodCard { Brand = "mastercard", Country = "US", ExpMonth = 11, ExpYear = 2024, Last4 = "1111" } }
            }
        };
        var paymentMethodModels = new List<PaymentMethodModel>
        {
            new PaymentMethodModel { Id = "pm_123", Type = "card", CreatedAt = paymentMethods.Data[0].Created, Card = new CardModel { Brand = "visa", Country = "US", ExpMonth = 12, ExpYear = 2025, Last4Digits = "4242" } },
            new PaymentMethodModel { Id = "pm_456", Type = "card", CreatedAt = paymentMethods.Data[1].Created, Card = new CardModel { Brand = "mastercard", Country = "US", ExpMonth = 11, ExpYear = 2024, Last4Digits = "1111" } }
        };
        _employersGrpcClientMock.Setup(c => c.GetEmployerByIdAsync(userId.ToString(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(employerDto);
        _customerPaymentMethodServiceMock.Setup(s => s.ListAsync("cus_123", null, It.IsAny<RequestOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(paymentMethods);
        _mapperMock.Setup(m => m.Map<PaymentMethodModel>(paymentMethods.Data[0])).Returns(paymentMethodModels[0]);
        _mapperMock.Setup(m => m.Map<PaymentMethodModel>(paymentMethods.Data[1])).Returns(paymentMethodModels[1]);

        // Act
        var result = await _service.GetPaymentMethodsAsync(userId, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(paymentMethodModels);
        _employersGrpcClientMock.Verify(c => c.GetEmployerByIdAsync(userId.ToString(), It.IsAny<CancellationToken>()), Times.Once());
        _customerPaymentMethodServiceMock.Verify(s => s.ListAsync("cus_123", null, It.IsAny<RequestOptions>(), It.IsAny<CancellationToken>()), Times.Once());
        _mapperMock.Verify(m => m.Map<PaymentMethodModel>(It.IsAny<PaymentMethod>()), Times.Exactly(2));
        _loggerMock.VerifyLog(LogLevel.Information, $"Getting payment methods for user {userId}", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Listing payment methods for customer {employerDto.EmployerCustomerId}", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Retrieved {paymentMethods.Data.Count} payment methods for user {userId}", Times.Once());
    }

    [Fact]
    public async Task GetPaymentMethodsAsync_NoEmployerAccount_ThrowsNotFoundException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var employerDto = new EmployerDto { Id = userId.ToString(), EmployerCustomerId = null };
        _employersGrpcClientMock.Setup(c => c.GetEmployerByIdAsync(userId.ToString(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(employerDto);

        // Act
        var act = async () => await _service.GetPaymentMethodsAsync(userId, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>().WithMessage($"Employer account by employer ID '{userId}' not found.");
        _customerPaymentMethodServiceMock.Verify(s => s.ListAsync(It.IsAny<string>(), null, It.IsAny<RequestOptions>(), It.IsAny<CancellationToken>()), Times.Never());
        _loggerMock.VerifyLog(LogLevel.Warning, $"Employer account not found for user {userId}", Times.Once());
    }

    [Fact]
    public async Task GetPaymentMethodsAsync_EmptyList_ReturnsEmptyList()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var employerDto = new EmployerDto { Id = userId.ToString(), EmployerCustomerId = "cus_123" };
        var paymentMethods = new StripeList<PaymentMethod> { Data = new List<PaymentMethod>() };
        _employersGrpcClientMock.Setup(c => c.GetEmployerByIdAsync(userId.ToString(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(employerDto);
        _customerPaymentMethodServiceMock.Setup(s => s.ListAsync("cus_123", null, It.IsAny<RequestOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(paymentMethods);

        // Act
        var result = await _service.GetPaymentMethodsAsync(userId, CancellationToken.None);

        // Assert
        result.Should().BeEmpty();
        _mapperMock.Verify(m => m.Map<PaymentMethodModel>(It.IsAny<PaymentMethod>()), Times.Never());
        _loggerMock.VerifyLog(LogLevel.Information, $"Retrieved 0 payment methods for user {userId}", Times.Once());
    }

    [Fact]
    public async Task GetPaymentMethodsAsync_StripeException_ThrowsBadRequestException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var employerDto = new EmployerDto { Id = userId.ToString(), EmployerCustomerId = "cus_123" };
        var stripeException = new StripeException("Stripe error");
        _employersGrpcClientMock.Setup(c => c.GetEmployerByIdAsync(userId.ToString(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(employerDto);
        _customerPaymentMethodServiceMock.Setup(s => s.ListAsync("cus_123", null, It.IsAny<RequestOptions>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(stripeException);

        // Act
        var act = async () => await _service.GetPaymentMethodsAsync(userId, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<BadRequestException>().WithMessage("Stripe error: Stripe error");
        _loggerMock.VerifyLog(LogLevel.Error, $"Stripe error getting payment methods: {stripeException.Message}", Times.Once());
    }

    [Fact]
    public async Task DeletePaymentMethodAsync_ValidInput_DeletesPaymentMethod()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var paymentMethodId = "pm_123";
        var employerDto = new EmployerDto { Id = userId.ToString(), EmployerCustomerId = "cus_123" };
        var paymentMethods = new StripeList<PaymentMethod>
        {
            Data = new List<PaymentMethod>
            {
                new PaymentMethod { Id = paymentMethodId }
            }
        };
        var detachedPaymentMethod = new PaymentMethod { Id = paymentMethodId };
        _employersGrpcClientMock.Setup(c => c.GetEmployerByIdAsync(userId.ToString(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(employerDto);
        _customerPaymentMethodServiceMock.Setup(s => s.ListAsync("cus_123", null, It.IsAny<RequestOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(paymentMethods);
        _paymentMethodServiceMock.Setup(s => s.DetachAsync(paymentMethodId, null, It.IsAny<RequestOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(detachedPaymentMethod);

        // Act
        await _service.DeletePaymentMethodAsync(userId, paymentMethodId, CancellationToken.None);

        // Assert
        _employersGrpcClientMock.Verify(c => c.GetEmployerByIdAsync(userId.ToString(), It.IsAny<CancellationToken>()), Times.Once());
        _customerPaymentMethodServiceMock.Verify(s => s.ListAsync("cus_123", null, It.IsAny<RequestOptions>(), It.IsAny<CancellationToken>()), Times.Once());
        _paymentMethodServiceMock.Verify(s => s.DetachAsync(paymentMethodId, null, It.IsAny<RequestOptions>(), It.IsAny<CancellationToken>()), Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Deleting payment method {paymentMethodId} for user {userId}", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Listing payment methods for customer {employerDto.EmployerCustomerId}", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Detaching payment method {paymentMethodId}", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, $"Payment method {paymentMethodId} deleted successfully for user {userId}", Times.Once());
    }

    [Fact]
    public async Task DeletePaymentMethodAsync_NoEmployerAccount_ThrowsNotFoundException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var paymentMethodId = "pm_123";
        var employerDto = new EmployerDto { Id = userId.ToString(), EmployerCustomerId = null };
        _employersGrpcClientMock.Setup(c => c.GetEmployerByIdAsync(userId.ToString(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(employerDto);

        // Act
        var act = async () => await _service.DeletePaymentMethodAsync(userId, paymentMethodId, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>().WithMessage($"Employer account by employer ID '{userId}' not found.");
        _customerPaymentMethodServiceMock.Verify(s => s.ListAsync(It.IsAny<string>(), null, It.IsAny<RequestOptions>(), It.IsAny<CancellationToken>()), Times.Never());
        _paymentMethodServiceMock.Verify(s => s.DetachAsync(It.IsAny<string>(), null, It.IsAny<RequestOptions>(), It.IsAny<CancellationToken>()), Times.Never());
        _loggerMock.VerifyLog(LogLevel.Warning, $"Employer account not found for user {userId}", Times.Once());
    }

    [Fact]
    public async Task DeletePaymentMethodAsync_PaymentMethodNotFound_ThrowsNotFoundException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var paymentMethodId = "pm_123";
        var employerDto = new EmployerDto { Id = userId.ToString(), EmployerCustomerId = "cus_123" };
        var paymentMethods = new StripeList<PaymentMethod> { Data = new List<PaymentMethod>() };
        _employersGrpcClientMock.Setup(c => c.GetEmployerByIdAsync(userId.ToString(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(employerDto);
        _customerPaymentMethodServiceMock.Setup(s => s.ListAsync("cus_123", null, It.IsAny<RequestOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(paymentMethods);

        // Act
        var act = async () => await _service.DeletePaymentMethodAsync(userId, paymentMethodId, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<BadRequestException>().WithMessage($"Could not delete Payment method with ID '{paymentMethodId}'.");
        _paymentMethodServiceMock.Verify(s => s.DetachAsync(It.IsAny<string>(), null, It.IsAny<RequestOptions>(), It.IsAny<CancellationToken>()), Times.Never());
        _loggerMock.VerifyLog(LogLevel.Error, $"Payment method {paymentMethodId} not found for user {userId}", Times.Once());
    }

    [Fact]
    public async Task DeletePaymentMethodAsync_StripeException_ThrowsBadRequestException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var paymentMethodId = "pm_123";
        var employerDto = new EmployerDto { Id = userId.ToString(), EmployerCustomerId = "cus_123" };
        var paymentMethods = new StripeList<PaymentMethod>
        {
            Data = new List<PaymentMethod>
            {
                new PaymentMethod { Id = paymentMethodId }
            }
        };
        var stripeException = new StripeException("Stripe error");
        _employersGrpcClientMock.Setup(c => c.GetEmployerByIdAsync(userId.ToString(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(employerDto);
        _customerPaymentMethodServiceMock.Setup(s => s.ListAsync("cus_123", null, It.IsAny<RequestOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(paymentMethods);
        _paymentMethodServiceMock.Setup(s => s.DetachAsync(paymentMethodId, null, It.IsAny<RequestOptions>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(stripeException);

        // Act
        var act = async () => await _service.DeletePaymentMethodAsync(userId, paymentMethodId, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<BadRequestException>().WithMessage("Stripe error: Stripe error");
        _loggerMock.VerifyLog(LogLevel.Error, $"Stripe error deleting payment method: {stripeException.Message}", Times.Once());
    }
}
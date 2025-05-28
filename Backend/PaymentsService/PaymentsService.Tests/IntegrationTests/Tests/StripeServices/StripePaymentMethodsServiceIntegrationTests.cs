using Microsoft.Extensions.DependencyInjection;
using PaymentsService.Application.Exceptions;
using PaymentsService.Domain.Abstractions.PaymentsServices;
using PaymentsService.Infrastructure.DTOs;
using PaymentsService.Tests.IntegrationTests.Helpers;
using Stripe;

namespace PaymentsService.Tests.IntegrationTests.Tests.StripeServices;

public class StripePaymentMethodsServiceIntegrationTests(
    StripeIntegrationTestsFixture fixture) : IClassFixture<StripeIntegrationTestsFixture>
{
    [Fact]
    public async Task SavePaymentMethodAsync_ValidData_ShouldSavePaymentMethod()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var customerService = new CustomerService();
        var customer = await customerService.CreateAsync(new CustomerCreateOptions { Email = "test@example.com" });
        var paymentMethodService = new PaymentMethodService();
        var paymentMethod = await paymentMethodService.CreateAsync(new PaymentMethodCreateOptions
        {
            Type = "card",
            Card = new PaymentMethodCardOptions { Token = "tok_visa" }
        });

        var employerDto = new EmployerDto { Id = userId.ToString(), EmployerCustomerId = customer.Id };

        fixture.EmployersGrpcClientMock
            .Setup(c => c.GetEmployerByIdAsync(userId.ToString(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(employerDto);

        using var scope = fixture.Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IPaymentMethodsService>();

        // Act
        await service.SavePaymentMethodAsync(userId, paymentMethod.Id, CancellationToken.None);

        // Assert
        var customerPaymentMethodService = new CustomerPaymentMethodService();
        var attachedMethod = await customerPaymentMethodService.GetAsync(customer.Id, paymentMethod.Id, cancellationToken: CancellationToken.None);
        attachedMethod.Should().NotBeNull();
        attachedMethod.Id.Should().Be(paymentMethod.Id);
        attachedMethod.CustomerId.Should().Be(customer.Id);
        await paymentMethodService.DetachAsync(paymentMethod.Id);
        await customerService.DeleteAsync(customer.Id);
    }

    [Fact]
    public async Task SavePaymentMethodAsync_NoEmployerAccount_ShouldThrowNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var paymentMethodId = "pm_test";
        var employerDto = new EmployerDto { Id = userId.ToString(), EmployerCustomerId = null };

        fixture.EmployersGrpcClientMock
            .Setup(c => c.GetEmployerByIdAsync(userId.ToString(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(employerDto);

        using var scope = fixture.Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IPaymentMethodsService>();

        // Act
        var act = async () => await service.SavePaymentMethodAsync(userId, paymentMethodId, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Employer account by employer ID '{userId}' not found.");
    }

    [Fact]
    public async Task SavePaymentMethodAsync_NonExistentPaymentMethod_ShouldThrowNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var paymentMethodId = "pm_nonexistent";
        var customerService = new CustomerService();
        var customer = await customerService.CreateAsync(new CustomerCreateOptions { Email = "test@example.com" });
        var employerDto = new EmployerDto { Id = userId.ToString(), EmployerCustomerId = customer.Id };

        fixture.EmployersGrpcClientMock
            .Setup(c => c.GetEmployerByIdAsync(userId.ToString(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(employerDto);

        using var scope = fixture.Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IPaymentMethodsService>();

        // Act
        var act = async () => await service.SavePaymentMethodAsync(userId, paymentMethodId, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<BadRequestException>();
        await customerService.DeleteAsync(customer.Id);
    }

    [Fact]
    public async Task GetPaymentMethodsAsync_ValidData_ShouldReturnPaymentMethods()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var customerService = new CustomerService();
        var customer = await customerService.CreateAsync(new CustomerCreateOptions { Email = "test@example.com" });
        var paymentMethodService = new PaymentMethodService();
        var paymentMethod1 = await paymentMethodService.CreateAsync(new PaymentMethodCreateOptions
        {
            Type = "card",
            Card = new PaymentMethodCardOptions { Token = "tok_visa" }
        });
        var paymentMethod2 = await paymentMethodService.CreateAsync(new PaymentMethodCreateOptions
        {
            Type = "card",
            Card = new PaymentMethodCardOptions { Token = "tok_mastercard" }
        });
        await paymentMethodService.AttachAsync(paymentMethod1.Id, new PaymentMethodAttachOptions { Customer = customer.Id });
        await paymentMethodService.AttachAsync(paymentMethod2.Id, new PaymentMethodAttachOptions { Customer = customer.Id });

        var employerDto = new EmployerDto { Id = userId.ToString(), EmployerCustomerId = customer.Id };

        fixture.EmployersGrpcClientMock
            .Setup(c => c.GetEmployerByIdAsync(userId.ToString(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(employerDto);

        using var scope = fixture.Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IPaymentMethodsService>();

        // Act
        var paymentMethods = await service.GetPaymentMethodsAsync(userId, CancellationToken.None);

        // Assert
        var paymentMethodModels = paymentMethods.ToList();
        paymentMethodModels.Should().HaveCount(2);
        paymentMethodModels.Should().Contain(pm => pm.Id == paymentMethod1.Id);
        paymentMethodModels.Should().Contain(pm => pm.Id == paymentMethod2.Id);
        await paymentMethodService.DetachAsync(paymentMethod1.Id);
        await paymentMethodService.DetachAsync(paymentMethod2.Id);
        await customerService.DeleteAsync(customer.Id);
    }

    [Fact]
    public async Task GetPaymentMethodsAsync_NoEmployerAccount_ShouldThrowNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var employerDto = new EmployerDto { Id = userId.ToString(), EmployerCustomerId = null };

        fixture.EmployersGrpcClientMock
            .Setup(c => c.GetEmployerByIdAsync(userId.ToString(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(employerDto);

        using var scope = fixture.Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IPaymentMethodsService>();

        // Act
        var act = async () => await service.GetPaymentMethodsAsync(userId, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Employer account by employer ID '{userId}' not found.");
    }

    [Fact]
    public async Task GetPaymentMethodsAsync_NoPaymentMethods_ShouldReturnEmptyList()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var customerService = new CustomerService();
        var customer = await customerService.CreateAsync(new CustomerCreateOptions { Email = "test@example.com" });
        var employerDto = new EmployerDto { Id = userId.ToString(), EmployerCustomerId = customer.Id };

        fixture.EmployersGrpcClientMock
            .Setup(c => c.GetEmployerByIdAsync(userId.ToString(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(employerDto);

        using var scope = fixture.Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IPaymentMethodsService>();

        // Act
        var paymentMethods = await service.GetPaymentMethodsAsync(userId, CancellationToken.None);

        // Assert
        paymentMethods.Should().BeEmpty();
        await customerService.DeleteAsync(customer.Id);
    }

    [Fact]
    public async Task DeletePaymentMethodAsync_ValidData_ShouldDeletePaymentMethod()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var customerService = new CustomerService();
        var customer = await customerService.CreateAsync(new CustomerCreateOptions { Email = "test@example.com" });
        var paymentMethodService = new PaymentMethodService();
        var paymentMethod = await paymentMethodService.CreateAsync(new PaymentMethodCreateOptions
        {
            Type = "card",
            Card = new PaymentMethodCardOptions { Token = "tok_visa" }
        });
        await paymentMethodService.AttachAsync(paymentMethod.Id, new PaymentMethodAttachOptions { Customer = customer.Id });

        var employerDto = new EmployerDto { Id = userId.ToString(), EmployerCustomerId = customer.Id };

        fixture.EmployersGrpcClientMock
            .Setup(c => c.GetEmployerByIdAsync(userId.ToString(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(employerDto);

        using var scope = fixture.Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IPaymentMethodsService>();

        // Act
        await service.DeletePaymentMethodAsync(userId, paymentMethod.Id, CancellationToken.None);

        // Assert
        var customerPaymentMethodService = new CustomerPaymentMethodService();
        var act = async () => await customerPaymentMethodService.GetAsync(customer.Id, paymentMethod.Id, cancellationToken: CancellationToken.None);
        await act.Should().ThrowAsync<StripeException>()
            .WithMessage("Invalid request");
        await customerService.DeleteAsync(customer.Id);
    }

    [Fact]
    public async Task DeletePaymentMethodAsync_NoEmployerAccount_ShouldThrowNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var paymentMethodId = "pm_test";
        var employerDto = new EmployerDto { Id = userId.ToString(), EmployerCustomerId = null };

        fixture.EmployersGrpcClientMock
            .Setup(c => c.GetEmployerByIdAsync(userId.ToString(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(employerDto);

        using var scope = fixture.Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IPaymentMethodsService>();

        // Act
        var act = async () => await service.DeletePaymentMethodAsync(userId, paymentMethodId, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Employer account by employer ID '{userId}' not found.");
    }

    [Fact]
    public async Task DeletePaymentMethodAsync_NonExistentPaymentMethod_ShouldThrowNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var paymentMethodId = "pm_nonexistent";
        var customerService = new CustomerService();
        var customer = await customerService.CreateAsync(new CustomerCreateOptions { Email = "test@example.com" });
        var employerDto = new EmployerDto { Id = userId.ToString(), EmployerCustomerId = customer.Id };

        fixture.EmployersGrpcClientMock
            .Setup(c => c.GetEmployerByIdAsync(userId.ToString(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(employerDto);

        using var scope = fixture.Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IPaymentMethodsService>();

        // Act
        var act = async () => await service.DeletePaymentMethodAsync(userId, paymentMethodId, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<BadRequestException>()
            .WithMessage($"Could not delete Payment method with ID '{paymentMethodId}'.");

        // Cleanup
        await customerService.DeleteAsync(customer.Id);
    }
}
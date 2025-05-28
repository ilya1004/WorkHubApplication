using Microsoft.Extensions.DependencyInjection;
using PaymentsService.Application.Exceptions;
using PaymentsService.Domain.Abstractions.PaymentsServices;
using PaymentsService.Domain.Models;
using PaymentsService.Infrastructure.DTOs;
using PaymentsService.Tests.IntegrationTests.Helpers;
using Stripe;

namespace PaymentsService.Tests.IntegrationTests.Tests.StripeServices;

public class StripeEmployerPaymentsServiceIntegrationTests(
    StripeIntegrationTestsFixture fixture) : IClassFixture<StripeIntegrationTestsFixture>
{
    [Fact]
    public async Task CreatePaymentIntentWithSavedMethodAsync_ValidData_ShouldCreatePaymentIntent()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var projectId = Guid.NewGuid();
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
        var projectDto = new ProjectDto { Id = projectId, BudgetInCents = 10000 };

        fixture.EmployersGrpcClientMock
            .Setup(c => c.GetEmployerByIdAsync(userId.ToString(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(employerDto);
        fixture.ProjectsGrpcClientMock
            .Setup(c => c.GetProjectByIdAsync(projectId.ToString(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(projectDto);
        fixture.PaymentsProducerServiceMock
            .Setup(p => p.SavePaymentIntentIdAsync(projectId.ToString(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        using var scope = fixture.Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IEmployerPaymentsService>();

        // Act
        await service.CreatePaymentIntentWithSavedMethodAsync(userId, projectId, paymentMethod.Id, CancellationToken.None);

        // Assert
        fixture.PaymentsProducerServiceMock.Verify(
            p => p.SavePaymentIntentIdAsync(projectId.ToString(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Once());
        await paymentMethodService.DetachAsync(paymentMethod.Id);
        await customerService.DeleteAsync(customer.Id);
    }

    [Fact]
    public async Task CreatePaymentIntentWithSavedMethodAsync_NoEmployerAccount_ShouldThrowNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var paymentMethodId = "pm_test";
        var employerDto = new EmployerDto { Id = userId.ToString(), EmployerCustomerId = null };

        fixture.EmployersGrpcClientMock
            .Setup(c => c.GetEmployerByIdAsync(userId.ToString(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(employerDto);

        using var scope = fixture.Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IEmployerPaymentsService>();

        // Act
        Func<Task> act = async () => await service.CreatePaymentIntentWithSavedMethodAsync(
            userId, projectId, paymentMethodId, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Employer account by employer ID '{userId}' not found.");
    }

    [Fact]
    public async Task ConfirmPaymentForProjectAsync_ValidPaymentIntent_ShouldConfirmAndTransfer()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var freelancerId = Guid.NewGuid();
        var customerService = new CustomerService();
        var customer = await customerService.CreateAsync(new CustomerCreateOptions { Email = "test@example.com" });
        var paymentMethodService = new PaymentMethodService();
        var paymentMethod = await paymentMethodService.CreateAsync(new PaymentMethodCreateOptions
        {
            Type = "card",
            Card = new PaymentMethodCardOptions { Token = "tok_visa" }
        });
        await paymentMethodService.AttachAsync(paymentMethod.Id, new PaymentMethodAttachOptions { Customer = customer.Id });

        var paymentIntentService = new PaymentIntentService();
        var paymentIntent = await paymentIntentService.CreateAsync(new PaymentIntentCreateOptions
        {
            Amount = 10000,
            Currency = "eur",
            Customer = customer.Id,
            CaptureMethod = "manual",
            Confirm = true,
            PaymentMethod = paymentMethod.Id,
            AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
            {
                Enabled = true,
                AllowRedirects = "never"
            }
        });

        var accountService = new AccountService();
        var freelancerAccount = await accountService.CreateAsync(new AccountCreateOptions
        {
            Type = "custom",
            Email = "test@test.com",
            BusinessType = "individual",
            Country = "LT",
            Individual = new AccountIndividualOptions
            {
                FirstName = "Test",
                LastName = "Test",
                Email = "test@test.com",
                Phone = "+37061234567",
                Address = new AddressOptions
                {
                    City = "Vilnius",
                    Line1 = "Test Street 123",
                    Country = "LT",
                    PostalCode = "LT-01100"
                },
                Dob = new DobOptions
                {
                    Day = 1,
                    Month = 1,
                    Year = 1990
                },
            },
            BusinessProfile = new AccountBusinessProfileOptions
            {
                Name = "WorkHub",
                Url = "https://www.workhub.me",
                Mcc = "7372"
            },
            Capabilities = new AccountCapabilitiesOptions
            {
                Transfers = new AccountCapabilitiesTransfersOptions { Requested = true },
                CardPayments = new AccountCapabilitiesCardPaymentsOptions { Requested = true }
            },
            ExternalAccount = new AccountBankAccountOptions
            {
                AccountNumber = "LT121000011101001000",
                Country = "LT",
                Currency = "eur",
            },
            TosAcceptance = new AccountTosAcceptanceOptions
            {
                Date = DateTime.UtcNow,
                Ip = "127.0.0.1"
            }
        });

        var projectDto = new ProjectDto
        {
            Id = projectId,
            PaymentIntentId = paymentIntent.Id,
            FreelancerId = freelancerId,
            BudgetInCents = 10000
        };
        var freelancerDto = new FreelancerDto { Id = freelancerId.ToString(), StripeAccountId = freelancerAccount.Id };

        fixture.ProjectsGrpcClientMock
            .Setup(c => c.GetProjectByIdAsync(projectId.ToString(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(projectDto);
        fixture.FreelancersGrpcClientMock
            .Setup(c => c.GetFreelancerByIdAsync(freelancerId.ToString(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(freelancerDto);
        fixture.TransfersServiceMock
            .Setup(t => t.TransferFundsToFreelancer(It.IsAny<PaymentIntentModel>(), projectId, freelancerAccount.Id, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        using var scope = fixture.Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IEmployerPaymentsService>();

        // Act
        await service.ConfirmPaymentForProjectAsync(projectId, CancellationToken.None);

        // Assert
        fixture.TransfersServiceMock.Verify(
            t => t.TransferFundsToFreelancer(It.IsAny<PaymentIntentModel>(), projectId, freelancerAccount.Id, It.IsAny<CancellationToken>()),
            Times.Once());
        await paymentMethodService.DetachAsync(paymentMethod.Id);
        await customerService.DeleteAsync(customer.Id);
        await accountService.DeleteAsync(freelancerAccount.Id);
    }

    [Fact]
    public async Task ConfirmPaymentForProjectAsync_NoPaymentIntent_ShouldThrowNotFound()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var projectDto = new ProjectDto { Id = projectId, PaymentIntentId = null };

        fixture.ProjectsGrpcClientMock
            .Setup(c => c.GetProjectByIdAsync(projectId.ToString(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(projectDto);

        using var scope = fixture.Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IEmployerPaymentsService>();

        // Act
        Func<Task> act = async () => await service.ConfirmPaymentForProjectAsync(projectId, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("This project does not have an attached Payment Intent.");
    }

    [Fact]
    public async Task CancelPaymentIntentForProjectAsync_ValidIntent_ShouldCancel()
    {
        // Arrange
        var paymentIntentService = new PaymentIntentService();
        var paymentIntent = await paymentIntentService.CreateAsync(new PaymentIntentCreateOptions
        {
            Amount = 10000,
            Currency = "eur",
            PaymentMethod = "pm_card_visa",
            Confirm = true,
            CaptureMethod = "manual",
            AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
            {
                Enabled = true,
                AllowRedirects = "never"
            }
        });

        using var scope = fixture.Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IEmployerPaymentsService>();

        // Act
        await service.CancelPaymentIntentForProjectAsync(paymentIntent.Id, CancellationToken.None);

        // Assert
        var canceledIntent = await paymentIntentService.GetAsync(paymentIntent.Id);
        canceledIntent.Status.Should().Be("canceled");
    }

    [Fact]
    public async Task CancelPaymentIntentForProjectAsync_NonExistentIntent_ShouldThrowNotFound()
    {
        // Arrange
        var paymentIntentId = "pi_nonexistent";
        using var scope = fixture.Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IEmployerPaymentsService>();

        // Act
        var act = async () => await service.CancelPaymentIntentForProjectAsync(paymentIntentId, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<BadRequestException>()
            .WithMessage($"Stripe error: No such payment_intent: '{paymentIntentId}'");
    }
}
using PaymentsService.Domain.Abstractions.TransfersServices;
using PaymentsService.Domain.Models;

namespace PaymentsService.Infrastructure.Services.StripeTransfersServices;

public class StripeTransfersService(IMapper mapper) : ITransfersService
{
    private readonly ChargeService _chargeService = new();
    private readonly TransferService _transferService = new();

    public async Task TransferFundsToFreelancer(PaymentIntentModel paymentIntent, Guid projectId,
        string freelancerStripeAccountId, CancellationToken cancellationToken)
    {
        var transferOptions = new TransferCreateOptions
        {
            Amount = paymentIntent.Amount,
            Currency = paymentIntent.Currency,
            Destination = freelancerStripeAccountId,
            TransferGroup = projectId.ToString()
        };

        try
        {
            await _transferService.CreateAsync(transferOptions, cancellationToken: cancellationToken);
        }
        catch (StripeException ex)
        {
            throw new BadRequestException($"Stripe error: {ex.Message}");
        }
        catch
        {
            throw new BadRequestException($"Could not create Transfer with Payment intent ID '{paymentIntent.Id}'.");
        }
    }

    public async Task<IEnumerable<ChargeModel>> GetEmployerPaymentsAsync(
        Guid userId, Guid? projectId, CancellationToken cancellationToken)
    {
        var employerCustomerId = Guid.NewGuid().ToString(); // This data will be requested from Identity Service via gRPC

        if (employerCustomerId is null) throw new NotFoundException($"Employer account by employer ID '{userId}' not found.");

        var chargeListOptions = new ChargeListOptions
        {
            Customer = employerCustomerId
        };

        if (projectId is not null) chargeListOptions.TransferGroup = projectId.ToString();

        try
        {
            var charges = await _chargeService.ListAsync(chargeListOptions, cancellationToken: cancellationToken);

            return charges.Data.Select(mapper.Map<ChargeModel>);
        }
        catch (StripeException ex)
        {
            throw new BadRequestException($"Stripe error: {ex.Message}");
        }
        catch
        {
            throw new BadRequestException($"Could not get Payments by employer with ID '{userId}'.");
        }
    }

    public async Task<IEnumerable<TransferModel>> GetFreelancerTransfersAsync(
        Guid userId, Guid? projectId, CancellationToken cancellationToken)
    {
        var freelancerStripeAccountId = Guid.NewGuid().ToString(); // This data will be requested from Identity Service via gRPC

        var options = new TransferListOptions
        {
            Destination = freelancerStripeAccountId
        };

        if (projectId is not null) options.TransferGroup = projectId.ToString();

        try
        {
            var transfers = await _transferService.ListAsync(options, cancellationToken: cancellationToken);

            return transfers.Data.Select(mapper.Map<TransferModel>);
        }
        catch (StripeException ex)
        {
            throw new BadRequestException($"Stripe error: {ex.Message}");
        }
        catch
        {
            throw new BadRequestException($"Could not get Payments by freelancer with ID '{userId}'.");
        }
    }
}
using PaymentsService.Domain.Abstractions.TransfersServices;
using PaymentsService.Domain.Models;

namespace PaymentsService.Infrastructure.Services.StripeTransfersServices;

public class StripeTransfersService : ITransfersService
{
    private readonly TransferService _transferService = new();
    public async Task TransferFundsToFreelancer(PaymentIntentModel paymentIntent, string freelancerStripeAccountId, 
        CancellationToken cancellationToken)
    {
        var transferOptions = new TransferCreateOptions
        {
            Amount = paymentIntent.Amount,
            Currency = paymentIntent.Currency,
            Destination = freelancerStripeAccountId,
            TransferGroup = paymentIntent.TransferGroup
        };

        await _transferService.CreateAsync(transferOptions, cancellationToken: cancellationToken);
    }
}
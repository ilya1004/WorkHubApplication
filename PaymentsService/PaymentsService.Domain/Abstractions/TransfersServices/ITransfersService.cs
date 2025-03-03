using PaymentsService.Domain.Models;

namespace PaymentsService.Domain.Abstractions.TransfersServices;

public interface ITransfersService
{
    Task TransferFundsToFreelancer(PaymentIntentModel paymentIntent, string freelancerStripeAccountId,
        CancellationToken cancellationToken);
}
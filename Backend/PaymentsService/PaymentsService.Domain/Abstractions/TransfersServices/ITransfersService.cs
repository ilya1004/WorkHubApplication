using PaymentsService.Domain.Models;

namespace PaymentsService.Domain.Abstractions.TransfersServices;

public interface ITransfersService
{
    Task TransferFundsToFreelancer(PaymentIntentModel paymentIntent, Guid projectId, string freelancerStripeAccountId,
        CancellationToken cancellationToken);

    Task<IEnumerable<ChargeModel>> GetEmployerPaymentsAsync(Guid userId, Guid? projectId,
        CancellationToken cancellationToken);
    Task<IEnumerable<ChargeModel>> GetAllEmployerPaymentsAsync(CancellationToken cancellationToken);

    Task<IEnumerable<PaymentIntentModel>> GetEmployerPaymentIntentsAsync(Guid userId, Guid? projectId,
        CancellationToken cancellationToken);

    Task<IEnumerable<TransferModel>> GetFreelancerTransfersAsync(Guid userId, Guid? projectId,
        CancellationToken cancellationToken);
    Task<IEnumerable<TransferModel>> GetAllFreelancerTransfersAsync(CancellationToken cancellationToken);
}
using PaymentsService.Domain.Abstractions.TransfersServices;
using PaymentsService.Domain.Models;

namespace PaymentsService.Infrastructure.Services.StripeTransfersServices;

public class StripeTransfersService(IMapper mapper) : ITransfersService
{
    private readonly TransferService _transferService = new();
    private readonly ChargeService _chargeService = new();
        
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

    public async Task<IEnumerable<ChargeModel>> GetEmployerPaymentsAsync(Guid userId, CancellationToken cancellationToken)
    {
        var employerCustomerId = Guid.NewGuid().ToString(); // This data will be requested from Identity Service via gRPC
        
        if (employerCustomerId is null)
        {
            throw new NotFoundException($"Employer account by employer ID '{userId}' not found.");
        }
        
        var chargeListOptions = new ChargeListOptions
        {
            Customer = employerCustomerId,
        };

        var charges = await _chargeService.ListAsync(chargeListOptions, cancellationToken: cancellationToken);

        return charges.Data.Select(mapper.Map<ChargeModel>);
    }
}
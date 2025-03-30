using PaymentsService.Domain.Abstractions.PaymentsServices;
using PaymentsService.Domain.Models;

namespace PaymentsService.Application.UseCases.PaymentMethodUseCases.Queries.GetMyPaymentMethods;

public class GetMyPaymentMethodsQueryHandler(
    IPaymentMethodsService paymentMethodsService,
    IUserContext userContext,
    ILogger<GetMyPaymentMethodsQueryHandler> logger) : IRequestHandler<GetMyPaymentMethodsQuery, IEnumerable<PaymentMethodModel>>
{
    public async Task<IEnumerable<PaymentMethodModel>> Handle(GetMyPaymentMethodsQuery request, CancellationToken cancellationToken)
    {
        var userId = userContext.GetUserId();
        
        logger.LogInformation("Retrieving payment methods for user {UserId}", userId);

        var result = await paymentMethodsService.GetPaymentMethodsAsync(
            userId, cancellationToken);
            
        logger.LogInformation("Successfully retrieved payment methods for user {UserId}", userId);
            
        return result;
    }
}
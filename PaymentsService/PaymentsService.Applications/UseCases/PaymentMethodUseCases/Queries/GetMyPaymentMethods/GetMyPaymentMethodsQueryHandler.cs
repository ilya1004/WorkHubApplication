using PaymentsService.Domain.Abstractions.PaymentsServices;
using PaymentsService.Domain.Abstractions.UserContext;
using PaymentsService.Domain.Models;

namespace PaymentsService.Applications.UseCases.PaymentMethodUseCases.Queries.GetMyPaymentMethods;

public class GetMyPaymentMethodsQueryHandler(
    IPaymentMethodsService paymentMethodsService,
    IUserContext userContext) : IRequestHandler<GetMyPaymentMethodsQuery, IEnumerable<PaymentMethodModel>>
{
    public async Task<IEnumerable<PaymentMethodModel>> Handle(GetMyPaymentMethodsQuery request, CancellationToken cancellationToken)
    {
        var userId = userContext.GetUserId();
        
        var result = await paymentMethodsService.GetPaymentMethodsAsync(userId, cancellationToken);

        return result;
    }
}
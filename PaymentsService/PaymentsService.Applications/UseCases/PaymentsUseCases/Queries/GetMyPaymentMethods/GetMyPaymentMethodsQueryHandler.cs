using PaymentsService.Domain.Abstractions.PaymentsServices;
using PaymentsService.Domain.Abstractions.UserContext;

namespace PaymentsService.Applications.UseCases.PaymentsUseCases.Queries.GetMyPaymentMethods;

public class GetMyPaymentMethodsQueryHandler(
    IEmployerPaymentsService employerPaymentsService,
    IUserContext userContext) : IRequestHandler<GetMyPaymentMethodsQuery, IEnumerable<PaymentMethodDto>>
{
    public async Task<IEnumerable<PaymentMethodDto>> Handle(GetMyPaymentMethodsQuery request, CancellationToken cancellationToken)
    {
        var userId = userContext.GetUserId();
        
        var result = await employerPaymentsService.GetPaymentMethodsAsync(userId, cancellationToken);

        return result;
    }
}
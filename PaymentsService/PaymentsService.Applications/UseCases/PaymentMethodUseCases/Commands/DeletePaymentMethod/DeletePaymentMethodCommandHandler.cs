using PaymentsService.Domain.Abstractions.PaymentsServices;
using PaymentsService.Domain.Abstractions.UserContext;

namespace PaymentsService.Applications.UseCases.PaymentMethodUseCases.Commands.DeletePaymentMethod;

public class DeletePaymentMethodCommandHandler(
    IPaymentMethodsService paymentMethodsService,
    IUserContext userContext) : IRequestHandler<DeletePaymentMethodCommand>
{
    public async Task Handle(DeletePaymentMethodCommand request, CancellationToken cancellationToken)
    {
        var userId = userContext.GetUserId();
        
        await paymentMethodsService.DeletePaymentMethodAsync(userId, request.PaymentMethodId, cancellationToken);
    }
}
using PaymentsService.Domain.Abstractions.PaymentsServices;
using PaymentsService.Domain.Abstractions.UserContext;

namespace PaymentsService.Applications.UseCases.PaymentMethodUseCases.Commands.SavePaymentMethod;

public class SavePaymentMethodCommandHandler(
    IEmployerPaymentMethodsService employerPaymentMethodsService,
    IUserContext userContext) : IRequestHandler<SavePaymentMethodCommand>
{
    public async Task Handle(SavePaymentMethodCommand request, CancellationToken cancellationToken)
    {
        var userId = userContext.GetUserId();
        
        await employerPaymentMethodsService.SavePaymentMethodAsync(userId, request.PaymentMethodId, cancellationToken);
    }
}
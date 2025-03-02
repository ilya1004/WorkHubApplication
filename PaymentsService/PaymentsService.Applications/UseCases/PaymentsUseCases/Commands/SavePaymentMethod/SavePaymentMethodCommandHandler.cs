using PaymentsService.Domain.Abstractions.PaymentsServices;
using PaymentsService.Domain.Abstractions.UserContext;

namespace PaymentsService.Applications.UseCases.PaymentsUseCases.Commands.SavePaymentMethod;

public class SavePaymentMethodCommandHandler(
    IEmployerPaymentsService employerPaymentsService,
    IUserContext userContext) : IRequestHandler<SavePaymentMethodCommand>
{
    public async Task Handle(SavePaymentMethodCommand request, CancellationToken cancellationToken)
    {
        var userId = userContext.GetUserId();
        
        await employerPaymentsService.SavePaymentMethodAsync(userId, request.PaymentMethodId, cancellationToken);
    }
}
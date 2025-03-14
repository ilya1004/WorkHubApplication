using PaymentsService.Domain.Abstractions.PaymentsServices;
using PaymentsService.Domain.Abstractions.UserContext;

namespace PaymentsService.Applications.UseCases.PaymentsUseCases.Commands.PayForProjectWithSavedMethod;

public class PayForProjectWithSavedMethodCommandHandler(
    IEmployerPaymentsService employerPaymentsService,
    IUserContext userContext) : IRequestHandler<PayForProjectWithSavedMethodCommand>
{
    public async Task Handle(PayForProjectWithSavedMethodCommand request, CancellationToken cancellationToken)
    {
        var userId = userContext.GetUserId();

        await employerPaymentsService.CreatePaymentIntentWithSavedMethodAsync(userId, request.ProjectId,
            request.PaymentMethodId, cancellationToken);
    }
}
using PaymentsService.Domain.Abstractions.PaymentsServices;

namespace PaymentsService.Application.UseCases.PaymentMethodUseCases.Commands.SavePaymentMethod;

public class SavePaymentMethodCommandHandler(
    IPaymentMethodsService paymentMethodsService,
    IUserContext userContext,
    ILogger<SavePaymentMethodCommandHandler> logger) : IRequestHandler<SavePaymentMethodCommand>
{
    public async Task Handle(SavePaymentMethodCommand request, CancellationToken cancellationToken)
    {
        var userId = userContext.GetUserId();
        
        logger.LogInformation("Starting to save payment method {PaymentMethodId} for user {UserId}", 
            request.PaymentMethodId, userId);

        await paymentMethodsService.SavePaymentMethodAsync(userId, request.PaymentMethodId, cancellationToken);
            
        logger.LogInformation("Successfully saved payment method {PaymentMethodId} for user {UserId}", 
            request.PaymentMethodId, userId);
    }
}
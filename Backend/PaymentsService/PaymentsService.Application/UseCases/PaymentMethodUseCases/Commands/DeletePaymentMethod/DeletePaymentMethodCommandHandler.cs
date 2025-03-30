using PaymentsService.Domain.Abstractions.PaymentsServices;

namespace PaymentsService.Application.UseCases.PaymentMethodUseCases.Commands.DeletePaymentMethod;

public class DeletePaymentMethodCommandHandler(
    IPaymentMethodsService paymentMethodsService,
    IUserContext userContext,
    ILogger<DeletePaymentMethodCommandHandler> logger) : IRequestHandler<DeletePaymentMethodCommand>
{
    public async Task Handle(DeletePaymentMethodCommand request, CancellationToken cancellationToken)
    {
        var userId = userContext.GetUserId();
        
        logger.LogInformation("Starting to delete payment method {PaymentMethodId} for user {UserId}", 
            request.PaymentMethodId, userId);

        await paymentMethodsService.DeletePaymentMethodAsync(userId, request.PaymentMethodId, cancellationToken);
            
        logger.LogInformation("Successfully deleted payment method {PaymentMethodId} for user {UserId}", 
            request.PaymentMethodId, userId);
    }
}
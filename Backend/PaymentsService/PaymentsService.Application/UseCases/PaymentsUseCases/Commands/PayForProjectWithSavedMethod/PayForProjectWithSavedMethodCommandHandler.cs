using PaymentsService.Domain.Abstractions.PaymentsServices;

namespace PaymentsService.Application.UseCases.PaymentsUseCases.Commands.PayForProjectWithSavedMethod;

public class PayForProjectWithSavedMethodCommandHandler(
    IEmployerPaymentsService employerPaymentsService,
    IUserContext userContext,
    ILogger<PayForProjectWithSavedMethodCommandHandler> logger) : IRequestHandler<PayForProjectWithSavedMethodCommand>
{
    public async Task Handle(PayForProjectWithSavedMethodCommand request, CancellationToken cancellationToken)
    {
        var userId = userContext.GetUserId();
        
        logger.LogInformation("Processing payment for project {ProjectId} with saved method {PaymentMethodId} by user {UserId}", 
            request.ProjectId, request.PaymentMethodId, userId);

        await employerPaymentsService.CreatePaymentIntentWithSavedMethodAsync(
            userId, request.ProjectId, request.PaymentMethodId, cancellationToken);
            
        logger.LogInformation("Payment for project {ProjectId} with saved method {PaymentMethodId} processed successfully by user {UserId}", 
            request.ProjectId, request.PaymentMethodId, userId);
    }
}
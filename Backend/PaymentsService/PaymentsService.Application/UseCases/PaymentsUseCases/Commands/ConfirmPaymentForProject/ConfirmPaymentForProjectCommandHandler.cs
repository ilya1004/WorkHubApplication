using PaymentsService.Domain.Abstractions.PaymentsServices;

namespace PaymentsService.Application.UseCases.PaymentsUseCases.Commands.ConfirmPaymentForProject;

public class ConfirmPaymentForProjectCommandHandler(
    IEmployerPaymentsService employerPaymentsService,
    IUserContext userContext,
    ILogger<ConfirmPaymentForProjectCommandHandler> logger) : IRequestHandler<ConfirmPaymentForProjectCommand>
{
    public async Task Handle(ConfirmPaymentForProjectCommand request, CancellationToken cancellationToken)
    {
        var userId = userContext.GetUserId();
        
        logger.LogInformation("Confirming payment for project {ProjectId} by user {UserId}", 
            request.ProjectId, userId);

        await employerPaymentsService.ConfirmPaymentForProjectAsync(request.ProjectId, cancellationToken);
            
        logger.LogInformation("Payment for project {ProjectId} confirmed successfully by user {UserId}", 
            request.ProjectId, userId);
    }
}
using PaymentsService.Domain.Abstractions.PaymentsServices;

namespace PaymentsService.Application.UseCases.PaymentsUseCases.Commands.CreateSetupIntent;

public class CreateSetupIntentCommandHandler(
    IEmployerPaymentsService employerPaymentsService,
    IUserContext userContext,
    ILogger<CreateSetupIntentCommandHandler> logger) : IRequestHandler<CreateSetupIntentCommand, string>
{
    public async Task<string> Handle(CreateSetupIntentCommand request, CancellationToken cancellationToken)
    {
        var userId = userContext.GetUserId();
        
        logger.LogInformation("Creating setup intent for user {UserId}", userId);

        var clientSecret = await employerPaymentsService.CreateSetupIntent(userId, cancellationToken);
            
        logger.LogInformation("Setup intent created successfully for user {UserId}", userId);
        
        return clientSecret;
    }
}
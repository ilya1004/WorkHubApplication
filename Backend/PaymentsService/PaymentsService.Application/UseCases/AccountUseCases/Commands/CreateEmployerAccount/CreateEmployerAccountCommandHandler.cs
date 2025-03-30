using PaymentsService.Domain.Abstractions.AccountsServices;
using PaymentsService.Domain.Abstractions.KafkaProducerServices;

namespace PaymentsService.Application.UseCases.AccountUseCases.Commands.CreateEmployerAccount;

public class CreateEmployerAccountCommandHandler(
    IEmployerAccountsService employerAccountsService,
    IUserContext userContext,
    IAccountsProducerService accountsProducerService,
    ILogger<CreateEmployerAccountCommandHandler> logger) : IRequestHandler<CreateEmployerAccountCommand>
{
    public async Task Handle(CreateEmployerAccountCommand request, CancellationToken cancellationToken)
    {
        var userId = userContext.GetUserId();
        var userEmail = userContext.GetUserEmail();
        
        logger.LogInformation("Creating employer account for user {UserId} with email {UserEmail}", 
            userId, userEmail);

        var employerAccountId = await employerAccountsService.CreateEmployerAccountAsync(
            userId, userEmail, cancellationToken);
            
        logger.LogInformation("Successfully created employer account with ID {AccountId} for user {UserId}", 
            employerAccountId, userId);

        logger.LogInformation("Saving employer account ID {AccountId} to producer service for user {UserId}", 
            employerAccountId, userId);
        
        await accountsProducerService.SaveEmployerAccountIdAsync(userId.ToString(), employerAccountId, cancellationToken);
        
        logger.LogInformation("Successfully processed employer account creation for user {UserId}", userId);
    }
}

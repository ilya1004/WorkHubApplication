using PaymentsService.Domain.Abstractions.AccountsServices;
using PaymentsService.Domain.Abstractions.KafkaProducerServices;

namespace PaymentsService.Application.UseCases.AccountUseCases.Commands.CreateFreelancerAccount;

public class CreateFreelancerAccountCommandHandler(
    IFreelancerAccountsService freelancerAccountsService,
    IUserContext userContext,
    IAccountsProducerService accountsProducerService,
    ILogger<CreateFreelancerAccountCommandHandler> logger) : IRequestHandler<CreateFreelancerAccountCommand>
{
    public async Task Handle(CreateFreelancerAccountCommand request, CancellationToken cancellationToken)
    {
        var userId = userContext.GetUserId();
        var userEmail = userContext.GetUserEmail();
        
        logger.LogInformation("Creating freelancer account for user {UserId} with email {UserEmail}", 
            userId, userEmail);

        var freelancerAccountId = await freelancerAccountsService.CreateFreelancerAccountAsync(
            userId, userEmail, cancellationToken);
            
        logger.LogInformation("Successfully created freelancer account with ID {AccountId} for user {UserId}", 
            freelancerAccountId, userId);

        logger.LogInformation("Saving freelancer account ID {AccountId} to producer service for user {UserId}", 
            freelancerAccountId, userId);
        
        await accountsProducerService.SaveFreelancerAccountIdAsync(userId.ToString(), freelancerAccountId, cancellationToken);
        
        logger.LogInformation("Successfully processed freelancer account creation for user {UserId}", userId);
    }
}
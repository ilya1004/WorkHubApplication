using PaymentsService.Domain.Abstractions.AccountsServices;
using PaymentsService.Domain.Models;

namespace PaymentsService.Application.UseCases.AccountUseCases.Queries.GetFreelancerAccount;

public class GetFreelancerAccountQueryHandler(
    IFreelancerAccountsService freelancerAccountsService,
    IUserContext userContext,
    ILogger<GetFreelancerAccountQueryHandler> logger) : IRequestHandler<GetFreelancerAccountQuery, FreelancerAccountModel>
{
    public async Task<FreelancerAccountModel> Handle(GetFreelancerAccountQuery request, CancellationToken cancellationToken)
    {
        var userId = userContext.GetUserId();
        
        logger.LogInformation("Retrieving freelancer account for user {UserId}", userId);

        var freelancerAccount = await freelancerAccountsService.GetFreelancerAccountAsync(userId, cancellationToken);
        
        logger.LogInformation("Successfully retrieved freelancer account for user {UserId}", userId);
        
        return freelancerAccount;
    }
}
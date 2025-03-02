using PaymentsService.Domain.Abstractions.AccountsServices;
using PaymentsService.Domain.Abstractions.UserContext;

namespace PaymentsService.Applications.UseCases.AccountUseCases.Queries.GetFreelancerAccount;

public class GetFreelancerAccountQueryHandler(
    IFreelancerAccountsService freelancerAccountsService,
    IUserContext userContext) : IRequestHandler<GetFreelancerAccountQuery, FreelancerAccountDto>
{
    public async Task<FreelancerAccountDto> Handle(GetFreelancerAccountQuery request, CancellationToken cancellationToken)
    {
        var userId = userContext.GetUserId();
        
        var freelancerAccount = await freelancerAccountsService.GetFreelancerAccountAsync(userId);

        if (freelancerAccount is null)
        {
            throw new NotFoundException($"Freelancer account by owner ID '{userId}' not found");
        }

        return freelancerAccount;
    }
}
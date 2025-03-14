using PaymentsService.Application.Exceptions;
using PaymentsService.Domain.Abstractions.AccountsServices;
using PaymentsService.Domain.Abstractions.UserContext;
using PaymentsService.Domain.Models;

namespace PaymentsService.Application.UseCases.AccountUseCases.Queries.GetFreelancerAccount;

public class GetFreelancerAccountQueryHandler(
    IFreelancerAccountsService freelancerAccountsService,
    IUserContext userContext) : IRequestHandler<GetFreelancerAccountQuery, FreelancerAccountModel>
{
    public async Task<FreelancerAccountModel> Handle(GetFreelancerAccountQuery request,
        CancellationToken cancellationToken)
    {
        var userId = userContext.GetUserId();

        var freelancerAccount = await freelancerAccountsService.GetFreelancerAccountAsync(userId, cancellationToken);

        if (freelancerAccount is null) throw new NotFoundException($"Freelancer account by owner ID '{userId}' not found");

        return freelancerAccount;
    }
}
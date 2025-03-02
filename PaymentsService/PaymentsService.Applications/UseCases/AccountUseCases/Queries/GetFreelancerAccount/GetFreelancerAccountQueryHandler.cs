using PaymentsService.Domain.Abstractions.EmployerService;
using PaymentsService.Domain.Abstractions.UserContext;

namespace PaymentsService.Applications.UseCases.AccountUseCases.Queries.GetFreelancerAccount;

public class GetFreelancerAccountQueryHandler(
    IFreelancerService freelancerService,
    IUserContext userContext) : IRequestHandler<GetFreelancerAccountQuery, FreelancerAccountDto>
{
    public async Task<FreelancerAccountDto> Handle(GetFreelancerAccountQuery request, CancellationToken cancellationToken)
    {
        var userId = userContext.GetUserId();
        
        var freelancerAccount = await freelancerService.GetFreelancerAccountAsync(userId);

        if (freelancerAccount is null)
        {
            throw new NotFoundException($"Freelancer account by owner ID '{userId}' not found");
        }

        return freelancerAccount;
    }
}
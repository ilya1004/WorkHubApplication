using PaymentsService.Domain.Abstractions.AccountsServices;
using PaymentsService.Domain.Abstractions.KafkaProducerServices;
using PaymentsService.Domain.Abstractions.UserContext;

namespace PaymentsService.Application.UseCases.AccountUseCases.Commands.CreateFreelancerAccount;

public class CreateFreelancerAccountCommandHandler(
    IFreelancerAccountsService freelancerAccountsService,
    IUserContext userContext,
    IAccountsProducerService accountsProducerService) : IRequestHandler<CreateFreelancerAccountCommand>
{
    public async Task Handle(CreateFreelancerAccountCommand request, CancellationToken cancellationToken)
    {
        var userId = userContext.GetUserId();
        var userEmail = userContext.GetUserEmail();

        var freelancerAccountId = await freelancerAccountsService.CreateFreelancerAccountAsync(
            userId, userEmail, cancellationToken);

        await accountsProducerService.SaveFreelancerAccountIdAsync(userId.ToString(), freelancerAccountId, cancellationToken);
    }
}
using PaymentsService.Domain.Abstractions.AccountsServices;
using PaymentsService.Domain.Abstractions.UserContext;

namespace PaymentsService.Application.UseCases.AccountUseCases.Commands.CreateFreelancerAccount;

public class CreateFreelancerAccountCommandHandler(
    IFreelancerAccountsService freelancerAccountsService,
    IUserContext userContext) : IRequestHandler<CreateFreelancerAccountCommand>
{
    public async Task Handle(CreateFreelancerAccountCommand request, CancellationToken cancellationToken)
    {
        var userId = userContext.GetUserId();
        var userEmail = userContext.GetUserEmail();

        var freelancerAccountId = await freelancerAccountsService.CreateFreelancerAccountAsync(
            userId, userEmail, cancellationToken);
        // This data will be saved to Identity Service via gRPC
    }
}
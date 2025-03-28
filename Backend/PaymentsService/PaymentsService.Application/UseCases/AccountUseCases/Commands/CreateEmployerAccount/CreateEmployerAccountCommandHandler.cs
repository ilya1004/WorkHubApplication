using PaymentsService.Domain.Abstractions.AccountsServices;
using PaymentsService.Domain.Abstractions.KafkaProducerServices;
using PaymentsService.Domain.Abstractions.UserContext;

namespace PaymentsService.Application.UseCases.AccountUseCases.Commands.CreateEmployerAccount;

public class CreateEmployerAccountCommandHandler(
    IEmployerAccountsService employerAccountsService,
    IUserContext userContext,
    IAccountsProducerService accountsProducerService) : IRequestHandler<CreateEmployerAccountCommand>
{
    public async Task Handle(CreateEmployerAccountCommand request, CancellationToken cancellationToken)
    {
        var userId = userContext.GetUserId();
        var userEmail = userContext.GetUserEmail();

        var employerAccountId = await employerAccountsService.CreateEmployerAccountAsync(
            userId, userEmail, cancellationToken);

        await accountsProducerService.SaveEmployerAccountIdAsync(employerAccountId, cancellationToken);
    }
}
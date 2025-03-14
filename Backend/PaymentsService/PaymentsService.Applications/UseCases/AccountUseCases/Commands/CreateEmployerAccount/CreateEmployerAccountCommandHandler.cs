using PaymentsService.Domain.Abstractions.AccountsServices;
using PaymentsService.Domain.Abstractions.UserContext;

namespace PaymentsService.Applications.UseCases.AccountUseCases.Commands.CreateEmployerAccount;

public class CreateEmployerAccountCommandHandler(
    IEmployerAccountsService employerAccountsService,
    IUserContext userContext) : IRequestHandler<CreateEmployerAccountCommand>
{
    public async Task Handle(CreateEmployerAccountCommand request, CancellationToken cancellationToken)
    {
        var userId = userContext.GetUserId();
        var userEmail = userContext.GetUserEmail();

        var employerAccountId = await employerAccountsService.CreateEmployerAccountAsync(
            userId, userEmail, cancellationToken);
        // This data will be saved to Identity Service via gRPC
    }
}
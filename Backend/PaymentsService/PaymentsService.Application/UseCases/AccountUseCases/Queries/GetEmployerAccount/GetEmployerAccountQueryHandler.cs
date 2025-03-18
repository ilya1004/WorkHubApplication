using PaymentsService.Application.Exceptions;
using PaymentsService.Domain.Abstractions.AccountsServices;
using PaymentsService.Domain.Abstractions.UserContext;
using PaymentsService.Domain.Models;

namespace PaymentsService.Application.UseCases.AccountUseCases.Queries.GetEmployerAccount;

public class GetEmployerAccountQueryHandler(
    IEmployerAccountsService employerAccountsService,
    IUserContext userContext) : IRequestHandler<GetEmployerAccountQuery, EmployerAccountModel>
{
    public async Task<EmployerAccountModel> Handle(GetEmployerAccountQuery request, CancellationToken cancellationToken)
    {
        var userId = userContext.GetUserId();

        var employerAccount = await employerAccountsService.GetEmployerAccountAsync(userId, cancellationToken);

        if (employerAccount is null) throw new NotFoundException($"Employer account by owner ID '{userId}' not found");

        return employerAccount;
    }
}
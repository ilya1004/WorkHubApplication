using PaymentsService.Domain.Abstractions.AccountsServices;
using PaymentsService.Domain.Models;

namespace PaymentsService.Application.UseCases.AccountUseCases.Queries.GetEmployerAccount;

public class GetEmployerAccountQueryHandler(
    IEmployerAccountsService employerAccountsService,
    IUserContext userContext,
    ILogger<GetEmployerAccountQueryHandler> logger) : IRequestHandler<GetEmployerAccountQuery, EmployerAccountModel>
{
    public async Task<EmployerAccountModel> Handle(GetEmployerAccountQuery request, CancellationToken cancellationToken)
    {
        var userId = userContext.GetUserId();
        
        logger.LogInformation("Retrieving employer account for user {UserId}", userId);

        var employerAccount = await employerAccountsService.GetEmployerAccountAsync(userId, cancellationToken);
        
        logger.LogInformation("Successfully retrieved employer account for user {UserId}", userId);
        
        return employerAccount;
    }
}
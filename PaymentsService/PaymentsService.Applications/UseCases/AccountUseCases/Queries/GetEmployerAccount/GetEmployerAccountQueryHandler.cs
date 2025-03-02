using PaymentsService.Domain.Abstractions.EmployerService;
using PaymentsService.Domain.Abstractions.UserContext;

namespace PaymentsService.Applications.UseCases.AccountUseCases.Queries.GetEmployerAccount;

public class GetEmployerAccountQueryHandler(
    IEmployerService employerService,
    IUserContext userContext) : IRequestHandler<GetEmployerAccountQuery, EmployerAccountDto>
{
    public async Task<EmployerAccountDto> Handle(GetEmployerAccountQuery request, CancellationToken cancellationToken)
    {
        var userId = userContext.GetUserId();
        
        var employerAccount = await employerService.GetEmployerAccountAsync(userId);

        if (employerAccount is null)
        {
            throw new NotFoundException($"Employer account by owner ID '{userId}' not found");
        }

        return employerAccount;
    }
}
using MediatR;
using PaymentsService.Domain.Abstractions.AccountsService;
using PaymentsService.Domain.Abstractions.EmployerService;
using PaymentsService.Domain.DTOs;

namespace PaymentsService.Applications.UseCases.AccountUseCases.Queries;

public class GetEmployerAccountQueryHandler(
    IEmployerService employerService) : IRequestHandler<GetEmployerAccountQuery, EmployerAccountDto>
{
    public Task<EmployerAccountDto> Handle(GetEmployerAccountQuery request, CancellationToken cancellationToken)
    {


        var q = await employerService.GetEmployerAccountAsync();
        
        return 
    }
}
using PaymentsService.Domain.Models;

namespace PaymentsService.Applications.UseCases.AccountUseCases.Queries.GetEmployerAccount;

public sealed record GetEmployerAccountQuery : IRequest<EmployerAccountModel>;
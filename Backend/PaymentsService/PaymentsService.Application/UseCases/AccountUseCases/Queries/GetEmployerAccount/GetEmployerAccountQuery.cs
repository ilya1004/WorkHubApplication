using PaymentsService.Domain.Models;

namespace PaymentsService.Application.UseCases.AccountUseCases.Queries.GetEmployerAccount;

public sealed record GetEmployerAccountQuery : IRequest<EmployerAccountModel>;
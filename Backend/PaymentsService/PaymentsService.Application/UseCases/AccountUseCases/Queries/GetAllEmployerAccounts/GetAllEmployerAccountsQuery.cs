using PaymentsService.Application.Models;
using PaymentsService.Domain.Models;

namespace PaymentsService.Application.UseCases.AccountUseCases.Queries.GetAllEmployerAccounts;

public sealed record GetAllEmployerAccountsQuery(int PageNo, int PageSize) : IRequest<PaginatedResultModel<EmployerAccountModel>>;
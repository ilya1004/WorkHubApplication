using PaymentsService.Application.Models;
using PaymentsService.Domain.Models;

namespace PaymentsService.Application.UseCases.AccountUseCases.Queries.GetAllFreelancerAccounts;

public sealed record GetAllFreelancerAccountsQuery(int PageNo, int PageSize) : IRequest<PaginatedResultModel<FreelancerAccountModel>>;
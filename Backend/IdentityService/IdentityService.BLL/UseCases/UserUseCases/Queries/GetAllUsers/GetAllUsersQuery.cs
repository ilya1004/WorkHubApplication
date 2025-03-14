using IdentityService.BLL.Models;

namespace IdentityService.BLL.UseCases.UserUseCases.Queries.GetAllUsers;

public sealed record GetAllUsersQuery(int PageNo = 1, int PageSize = 10) : IRequest<PaginatedResultModel<AppUser>>;
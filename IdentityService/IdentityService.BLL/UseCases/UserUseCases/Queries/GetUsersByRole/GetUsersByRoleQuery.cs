using IdentityService.BLL.Models;

namespace IdentityService.BLL.UseCases.UserUseCases.Queries.GetUsersByRole;

public sealed record GetUsersByRoleQuery(string RoleName, int PageNo = 1, int PageSize = 10) : IRequest<PaginatedResultModel<AppUser>>;

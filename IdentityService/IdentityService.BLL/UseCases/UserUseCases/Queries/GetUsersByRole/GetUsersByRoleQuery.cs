namespace IdentityService.BLL.UseCases.UserUseCases.Queries.GetUsersByRole;

public sealed record GetUsersByRoleQuery(string UserRole, int PageNo = 1, int PageSize = 10) : IRequest<IEnumerable<AppUser>>;

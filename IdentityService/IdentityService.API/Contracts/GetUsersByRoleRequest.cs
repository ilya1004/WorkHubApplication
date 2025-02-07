namespace IdentityService.API.Contracts;

public sealed record GetUsersByRoleRequest(string UserRole, int PageNo = 1, int PageSize = 10);

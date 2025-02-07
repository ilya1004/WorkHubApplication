namespace IdentityService.API.Contracts.UserContracts;

public sealed record GetUsersByRoleRequest(string UserRole, int PageNo = 1, int PageSize = 10);
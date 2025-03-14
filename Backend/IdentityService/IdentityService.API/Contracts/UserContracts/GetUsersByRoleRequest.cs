namespace IdentityService.API.Contracts.UserContracts;

public sealed record GetUsersByRoleRequest(string RoleName, int PageNo = 1, int PageSize = 10);
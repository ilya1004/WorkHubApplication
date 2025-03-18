namespace IdentityService.API.Contracts.CommonContracts;

public sealed record GetPaginatedListRequest(int PageNo = 1, int PageSize = 10);
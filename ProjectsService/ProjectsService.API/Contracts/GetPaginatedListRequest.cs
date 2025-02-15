namespace ProjectsService.API.Contracts;

public sealed record GetPaginatedListRequest(int PageNo = 1, int PageSize = 10);
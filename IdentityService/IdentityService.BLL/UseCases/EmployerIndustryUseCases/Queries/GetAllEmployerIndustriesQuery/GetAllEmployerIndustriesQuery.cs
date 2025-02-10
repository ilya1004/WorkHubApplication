using IdentityService.BLL.Models;

namespace IdentityService.BLL.UseCases.EmployerIndustryUseCases.Queries.GetAllEmployerIndustriesQuery;

public sealed record GetAllEmployerIndustriesQuery(int PageNo = 1, int PageSize = 10) : IRequest<PaginatedResultModel<EmployerIndustry>>;
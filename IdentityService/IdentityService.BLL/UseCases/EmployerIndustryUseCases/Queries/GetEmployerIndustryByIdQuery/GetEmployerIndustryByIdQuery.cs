namespace IdentityService.BLL.UseCases.EmployerIndustryUseCases.Queries.GetEmployerIndustryByIdQuery;

public sealed record GetEmployerIndustryByIdQuery(Guid Id) : IRequest<EmployerIndustry>;
namespace IdentityService.BLL.UseCases.EmployerIndustryUseCases.Queries.GetEmployerIndustryById;

public sealed record GetEmployerIndustryByIdQuery(Guid Id) : IRequest<EmployerIndustry>;
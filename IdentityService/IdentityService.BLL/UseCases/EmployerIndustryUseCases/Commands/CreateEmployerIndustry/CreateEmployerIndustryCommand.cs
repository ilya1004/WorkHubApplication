namespace IdentityService.BLL.UseCases.EmployerIndustryUseCases.Commands.CreateEmployerIndustry;

public sealed record CreateEmployerIndustryCommand(string Name) : IRequest;
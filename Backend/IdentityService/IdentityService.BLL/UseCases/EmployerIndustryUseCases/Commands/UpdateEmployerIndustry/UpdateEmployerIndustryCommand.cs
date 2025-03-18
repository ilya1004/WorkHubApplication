namespace IdentityService.BLL.UseCases.EmployerIndustryUseCases.Commands.UpdateEmployerIndustry;

public sealed record UpdateEmployerIndustryCommand(Guid Id, string Name) : IRequest;
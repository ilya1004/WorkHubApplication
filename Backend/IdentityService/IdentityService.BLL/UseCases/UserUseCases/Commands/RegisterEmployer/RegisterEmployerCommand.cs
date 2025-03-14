namespace IdentityService.BLL.UseCases.UserUseCases.Commands.RegisterEmployer;

public sealed record RegisterEmployerCommand(string UserName, string CompanyName, string Email, string Password) : IRequest;
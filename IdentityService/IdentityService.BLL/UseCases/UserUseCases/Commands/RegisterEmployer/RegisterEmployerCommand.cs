using MediatR;

namespace IdentityService.BLL.UseCases.UserUseCases.Commands.RegisterEmployer;

public class RegisterEmployerCommand(string CompanyName, string Email, string Password) : IRequest;

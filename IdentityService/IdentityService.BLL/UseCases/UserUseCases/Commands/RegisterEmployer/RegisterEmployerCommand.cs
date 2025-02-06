using MediatR;

namespace IdentityService.BLL.UseCases.UserUseCases.Commands.RegisterEmployer;

public sealed record RegisterEmployerCommand(string CompanyName, string Email, string Password) : IRequest;

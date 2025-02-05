using MediatR;

namespace IdentityService.BLL.UseCases.UserUseCases.Commands.RegisterFreelancer;

public class RegisterFreelancerCommand(string UserName, string FirstName, string LastName, string Email, string Password) : IRequest;

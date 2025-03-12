namespace IdentityService.BLL.UseCases.UserUseCases.Commands.RegisterFreelancer;

public sealed record RegisterFreelancerCommand(string UserName, string FirstName, string LastName, string Email, string Password)
    : IRequest;
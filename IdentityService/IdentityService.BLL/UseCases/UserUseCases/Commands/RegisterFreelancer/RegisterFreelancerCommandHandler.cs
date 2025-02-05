using IdentityService.DAL.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace IdentityService.BLL.UseCases.UserUseCases.Commands.RegisterEmployer;

public class RegisterFreelancerCommandHandler : IRequestHandler<RegisterEmployerCommand>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly 
    public Task Handle(RegisterEmployerCommand request, CancellationToken cancellationToken)
    {
        
    }
}

using IdentityService.DAL.Abstractions.Data;
using IdentityService.DAL.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace IdentityService.BLL.UseCases.UserUseCases.Commands.RegisterEmployer;

public class RegisterEmployerCommandHandler : IRequestHandler<RegisterEmployerCommand>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterEmployerCommandHandler(UserManager<AppUser> userManager, IUnitOfWork unitOfWork)
    {
        _userManager = userManager;
        _unitOfWork = unitOfWork;
    }

    public Task Handle(RegisterEmployerCommand request, CancellationToken cancellationToken)
    {
        
    }
}

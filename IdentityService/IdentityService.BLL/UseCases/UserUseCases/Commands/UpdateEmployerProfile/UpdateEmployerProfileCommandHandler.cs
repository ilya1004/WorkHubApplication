namespace IdentityService.BLL.UseCases.UserUseCases.Commands.UpdateEmployerProfile;

public class UpdateEmployerProfileCommandHandler(
    UserManager<AppUser> userManager, 
    IUnitOfWork unitOfWork, 
    IMapper mapper) : IRequestHandler<UpdateEmployerProfileCommand>
{
    public async Task Handle(UpdateEmployerProfileCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Id.ToString());

        if (user == null)
        {
            throw new NotFoundException($"User with ID '{request.Id}' not found");
        }

        var employerProfile = mapper.Map<EmployerProfile>(request.EmployerProfile);

        // saving image

        await unitOfWork.EmployersRepository.UpdateAsync(employerProfile, cancellationToken);
        await unitOfWork.SaveAllAsync(cancellationToken);
    }
}

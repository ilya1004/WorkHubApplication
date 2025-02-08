
namespace IdentityService.BLL.UseCases.UserUseCases.Commands.UpdateFreelancerProfile;

public class UpdateFreelancerProfileCommandHandler(
    UserManager<AppUser> userManager, 
    IUnitOfWork unitOfWork,
    IMapper mapper) : IRequestHandler<UpdateFreelancerProfileCommand>
{
    public async Task Handle(UpdateFreelancerProfileCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Id.ToString());

        if (user == null)
        {
            throw new NotFoundException($"User with ID '{request.Id}' not found");
        }

        var freelancerProfile = mapper.Map<FreelancerProfile>(request.FreelancerProfile);

        freelancerProfile.Skills = [.. (await unitOfWork.FreelancerSkillsRepository.ListAsync(s => request.FreelancerProfile.SkillIds.Contains(s.Id), cancellationToken))];

        // saving image

        await unitOfWork.FreelancersRepository.UpdateAsync(freelancerProfile, cancellationToken);
        await unitOfWork.SaveAllAsync(cancellationToken);
    }
}

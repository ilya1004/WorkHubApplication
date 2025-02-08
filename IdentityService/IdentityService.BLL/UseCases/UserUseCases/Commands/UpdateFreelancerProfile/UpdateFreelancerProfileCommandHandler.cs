namespace IdentityService.BLL.UseCases.UserUseCases.Commands.UpdateFreelancerProfile;

public class UpdateFreelancerProfileCommandHandler( 
    IUnitOfWork unitOfWork,
    IMapper mapper) : IRequestHandler<UpdateFreelancerProfileCommand>
{
    public async Task Handle(UpdateFreelancerProfileCommand request, CancellationToken cancellationToken)
    {
        var user = await unitOfWork.UsersRepository.GetByIdAsync(
            request.Id, cancellationToken,
            u => u.FreelancerProfile!, u => u.FreelancerProfile!.Skills);

        if (user == null)
        {
            throw new NotFoundException($"User with ID '{request.Id}' not found");
        }

        mapper.Map(request.FreelancerProfile, user.FreelancerProfile);
        
        var skills = await unitOfWork.FreelancerSkillsRepository.ListAsync(
            s => request.FreelancerProfile.SkillIds.Contains(s.Id), cancellationToken);

        user.FreelancerProfile!.Skills = skills.ToList();

        await unitOfWork.UsersRepository.UpdateAsync(user, cancellationToken);
        await unitOfWork.SaveAllAsync(cancellationToken);
    }
}

namespace IdentityService.BLL.UseCases.FreelancerSkillUseCases.Commands.UpdateFreelancerSkill;

public class UpdateFreelancerSkillCommandHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper) : IRequestHandler<UpdateFreelancerSkillCommand>
{
    public async Task Handle(UpdateFreelancerSkillCommand request, CancellationToken cancellationToken)
    {
        var freelancerSkill = await unitOfWork.FreelancerSkillsRepository.GetByIdAsync(request.Id);

        if (freelancerSkill == null)
        {
            throw new NotFoundException($"Freelancer skill with ID '{request.Id}' not found");
        }

        mapper.Map(request, freelancerSkill);

        await unitOfWork.FreelancerSkillsRepository.UpdateAsync(freelancerSkill, cancellationToken);
        await unitOfWork.SaveAllAsync(cancellationToken);
    }
}

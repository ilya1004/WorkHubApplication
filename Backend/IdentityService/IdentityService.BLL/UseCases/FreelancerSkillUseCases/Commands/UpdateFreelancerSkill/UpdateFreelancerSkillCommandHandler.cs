namespace IdentityService.BLL.UseCases.FreelancerSkillUseCases.Commands.UpdateFreelancerSkill;

public class UpdateFreelancerSkillCommandHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ILogger<UpdateFreelancerSkillCommandHandler> logger) : IRequestHandler<UpdateFreelancerSkillCommand>
{
    public async Task Handle(UpdateFreelancerSkillCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Updating freelancer skill with ID: {SkillId}", request.Id);

        var freelancerSkill = await unitOfWork.FreelancerSkillsRepository.GetByIdAsync(request.Id, cancellationToken);

        if (freelancerSkill is null)
        {
            logger.LogWarning("Freelancer skill with ID {SkillId} not found", request.Id);
            
            throw new NotFoundException($"Freelancer skill with ID '{request.Id}' not found");
        }

        logger.LogInformation("Mapping changes to freelancer skill with ID: {SkillId}", request.Id);
        
        mapper.Map(request, freelancerSkill);

        await unitOfWork.FreelancerSkillsRepository.UpdateAsync(freelancerSkill, cancellationToken);
        await unitOfWork.SaveAllAsync(cancellationToken);
        
        logger.LogInformation("Successfully updated freelancer skill with ID: {SkillId}", request.Id);
    }
}
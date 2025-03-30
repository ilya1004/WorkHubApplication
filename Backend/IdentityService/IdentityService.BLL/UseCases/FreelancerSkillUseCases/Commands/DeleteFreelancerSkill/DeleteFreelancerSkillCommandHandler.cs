namespace IdentityService.BLL.UseCases.FreelancerSkillUseCases.Commands.DeleteFreelancerSkill;

public class DeleteFreelancerSkillCommandHandler(
    IUnitOfWork unitOfWork,
    ILogger<DeleteFreelancerSkillCommandHandler> logger) : IRequestHandler<DeleteFreelancerSkillCommand>
{
    public async Task Handle(DeleteFreelancerSkillCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Deleting freelancer skill with ID: {SkillId}", request.Id);

        var freelancerSkill = await unitOfWork.FreelancerSkillsRepository.GetByIdAsync(request.Id, cancellationToken);

        if (freelancerSkill is null)
        {
            logger.LogWarning("Freelancer skill with ID {SkillId} not found", request.Id);
            
            throw new NotFoundException($"Freelancer skill with ID '{request.Id}' not found");
        }

        await unitOfWork.FreelancerSkillsRepository.DeleteAsync(freelancerSkill, cancellationToken);
        await unitOfWork.SaveAllAsync(cancellationToken);
        
        logger.LogInformation("Successfully deleted freelancer skill with ID: {SkillId}", request.Id);
    }
}
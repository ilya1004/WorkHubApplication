namespace IdentityService.BLL.UseCases.FreelancerSkillUseCases.Queries.GetFreelancerSkillById;

public class GetFreelancerSkillByIdQueryHandler(
    IUnitOfWork unitOfWork,
    ILogger<GetFreelancerSkillByIdQueryHandler> logger) 
    : IRequestHandler<GetFreelancerSkillByIdQuery, FreelancerSkill>
{
    public async Task<FreelancerSkill> Handle(GetFreelancerSkillByIdQuery request, 
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting freelancer skill by ID: {SkillId}", request.Id);

        var freelancerSkill = await unitOfWork.FreelancerSkillsRepository.GetByIdAsync(request.Id, cancellationToken);

        if (freelancerSkill is null)
        {
            logger.LogWarning("Freelancer skill with ID {SkillId} not found", request.Id);
            
            throw new NotFoundException($"Freelancer Skill with ID '{request.Id}' not found");
        }

        logger.LogInformation("Successfully retrieved freelancer skill with ID: {SkillId}", request.Id);
        
        return freelancerSkill;
    }
}
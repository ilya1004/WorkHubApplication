namespace IdentityService.BLL.UseCases.FreelancerSkillUseCases.Commands.CreateFreelancerSkill;

public class CreateFreelancerSkillCommandHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ILogger<CreateFreelancerSkillCommandHandler> logger) : IRequestHandler<CreateFreelancerSkillCommand>
{
    public async Task Handle(CreateFreelancerSkillCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Creating new freelancer skill with name: {SkillName}", request.Name);

        var freelancerSkill =
            await unitOfWork.FreelancerSkillsRepository.FirstOrDefaultAsync(fs => fs.Name == request.Name, cancellationToken);

        if (freelancerSkill != null)
        {
            logger.LogWarning("Freelancer skill with name {SkillName} already exists", request.Name);
            
            throw new BadRequestException($"Freelancer skill with name '{request.Name}' already exists.");
        }

        logger.LogInformation("Mapping and creating new freelancer skill");
        
        var newFreelancerSkill = mapper.Map<FreelancerSkill>(request);

        await unitOfWork.FreelancerSkillsRepository.AddAsync(newFreelancerSkill, cancellationToken);
        await unitOfWork.SaveAllAsync(cancellationToken);
        
        logger.LogInformation("Successfully created freelancer skill with ID: {SkillId}", newFreelancerSkill.Id);
    }
}
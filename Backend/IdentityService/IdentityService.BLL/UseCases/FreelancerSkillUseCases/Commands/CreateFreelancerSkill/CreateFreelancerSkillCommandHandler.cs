using IdentityService.DAL.Abstractions.Repositories;

namespace IdentityService.BLL.UseCases.FreelancerSkillUseCases.Commands.CreateFreelancerSkill;

public class CreateFreelancerSkillCommandHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper) : IRequestHandler<CreateFreelancerSkillCommand>
{
    public async Task Handle(CreateFreelancerSkillCommand request, CancellationToken cancellationToken)
    {
        var freelancerSkill =
            await unitOfWork.FreelancerSkillsRepository.FirstOrDefaultAsync(fs => fs.Name == request.Name, cancellationToken);

        if (freelancerSkill != null) throw new BadRequestException($"Freelancer skill with name '{request.Name}' already exists.");

        var newFreelancerSkill = mapper.Map<FreelancerSkill>(request);

        await unitOfWork.FreelancerSkillsRepository.AddAsync(newFreelancerSkill, cancellationToken);
        await unitOfWork.SaveAllAsync(cancellationToken);
    }
}
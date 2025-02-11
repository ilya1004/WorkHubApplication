using IdentityService.DAL.Abstractions.Repositories;

namespace IdentityService.BLL.UseCases.FreelancerSkillUseCases.Queries.GetFreelancerSkillById;

public class GetFreelancerSkillByIdQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetFreelancerSkillByIdQuery, FreelancerSkill>
{
    public async Task<FreelancerSkill> Handle(GetFreelancerSkillByIdQuery request, CancellationToken cancellationToken)
    {
        var freelancerSkill = await unitOfWork.FreelancerSkillsRepository.GetByIdAsync(
            request.Id,
            cancellationToken);

        if (freelancerSkill is null)
        {
            throw new NotFoundException($"Freelancer Skill with ID '{request.Id}' not found");
        }

        return freelancerSkill;
    }
}

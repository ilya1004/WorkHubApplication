using IdentityService.BLL.Models;
using IdentityService.DAL.Abstractions.Repositories;

namespace IdentityService.BLL.UseCases.FreelancerSkillUseCases.Queries.GetAllFreelancerSkills;

public class GetAllFreelancerSkillsQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetAllFreelancerSkillsQuery, PaginatedResultModel<FreelancerSkill>>
{
    public async Task<PaginatedResultModel<FreelancerSkill>> Handle(GetAllFreelancerSkillsQuery request, CancellationToken cancellationToken)
    {
        var offset = (request.PageNo - 1) * request.PageSize;

        var skills = await unitOfWork.FreelancerSkillsRepository.PaginatedListAllAsync(
            offset,
            request.PageSize,
            cancellationToken);

        var skillsCount = await unitOfWork.FreelancerSkillsRepository.CountAllAsync(cancellationToken);

        return new PaginatedResultModel<FreelancerSkill>
        {
            Items = skills.ToList(),
            TotalCount = skillsCount,
            PageNo = request.PageNo,
            PageSize = request.PageSize
        };
    }
}

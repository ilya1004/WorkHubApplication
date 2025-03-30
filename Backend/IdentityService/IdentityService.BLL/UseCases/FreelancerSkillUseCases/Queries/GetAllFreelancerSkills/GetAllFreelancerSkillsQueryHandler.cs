using IdentityService.BLL.Models;

namespace IdentityService.BLL.UseCases.FreelancerSkillUseCases.Queries.GetAllFreelancerSkills;

public class GetAllFreelancerSkillsQueryHandler(
    IUnitOfWork unitOfWork,
    ILogger<GetAllFreelancerSkillsQueryHandler> logger) 
    : IRequestHandler<GetAllFreelancerSkillsQuery, PaginatedResultModel<FreelancerSkill>>
{
    public async Task<PaginatedResultModel<FreelancerSkill>> Handle(GetAllFreelancerSkillsQuery request, 
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting paginated list of freelancer skills. Page: {PageNo}, Size: {PageSize}", 
            request.PageNo, request.PageSize);

        var offset = (request.PageNo - 1) * request.PageSize;

        var skills = await unitOfWork.FreelancerSkillsRepository.PaginatedListAllAsync(
            offset, request.PageSize, cancellationToken);

        var skillsCount = await unitOfWork.FreelancerSkillsRepository.CountAllAsync(cancellationToken);

        logger.LogInformation("Retrieved {Count} skills out of {TotalCount}", skills.Count, skillsCount);

        return new PaginatedResultModel<FreelancerSkill>
        {
            Items = skills.ToList(),
            TotalCount = skillsCount,
            PageNo = request.PageNo,
            PageSize = request.PageSize
        };
    }
}
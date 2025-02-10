using IdentityService.BLL.Models;

namespace IdentityService.BLL.UseCases.FreelancerSkillUseCases.Queries.GetAllFreelancerSkills;

public sealed record GetAllFreelancerSkillsQuery(int PageNo = 1, int PageSize = 10) : IRequest<PaginatedResultModel<FreelancerSkill>>;
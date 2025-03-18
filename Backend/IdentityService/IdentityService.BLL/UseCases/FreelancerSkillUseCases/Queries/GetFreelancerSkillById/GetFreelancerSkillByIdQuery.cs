namespace IdentityService.BLL.UseCases.FreelancerSkillUseCases.Queries.GetFreelancerSkillById;

public sealed record GetFreelancerSkillByIdQuery(Guid Id) : IRequest<FreelancerSkill>;
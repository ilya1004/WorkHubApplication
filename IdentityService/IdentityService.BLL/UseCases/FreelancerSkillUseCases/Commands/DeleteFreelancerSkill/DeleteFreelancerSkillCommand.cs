namespace IdentityService.BLL.UseCases.FreelancerSkillUseCases.Commands.DeleteFreelancerSkill;

public sealed record DeleteFreelancerSkillCommand(Guid Id) : IRequest;
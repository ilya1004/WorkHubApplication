namespace IdentityService.BLL.UseCases.FreelancerSkillUseCases.Commands.UpdateFreelancerSkill;

public sealed record UpdateFreelancerSkillCommand(Guid Id, string Name) : IRequest;
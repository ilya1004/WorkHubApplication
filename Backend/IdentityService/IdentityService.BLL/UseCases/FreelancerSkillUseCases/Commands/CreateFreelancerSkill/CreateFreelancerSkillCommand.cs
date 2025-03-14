namespace IdentityService.BLL.UseCases.FreelancerSkillUseCases.Commands.CreateFreelancerSkill;

public sealed record CreateFreelancerSkillCommand(string Name) : IRequest;
using IdentityService.BLL.UseCases.FreelancerSkillUseCases.Commands.CreateFreelancerSkill;

namespace IdentityService.BLL.Mapping.FreelancerSkillMappingProfiles;

public class CreateFreelancerSkillCommandToFreelancerSkillProfile : Profile
{
    public CreateFreelancerSkillCommandToFreelancerSkillProfile()
    {
        CreateMap<CreateFreelancerSkillCommand, FreelancerSkill>()
            .ForMember(dest => dest.NormalizedName, opt
                => opt.MapFrom(src => src.Name.ToUpperInvariant().Replace(' ', '_')));
    }
}

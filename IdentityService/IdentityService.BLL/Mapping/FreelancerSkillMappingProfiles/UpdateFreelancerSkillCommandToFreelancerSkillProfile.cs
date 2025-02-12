using IdentityService.BLL.UseCases.FreelancerSkillUseCases.Commands.UpdateFreelancerSkill;

namespace IdentityService.BLL.Mapping.FreelancerSkillMappingProfiles;

public class UpdateFreelancerSkillCommandToFreelancerSkillProfile : Profile
{
    public UpdateFreelancerSkillCommandToFreelancerSkillProfile()
    {
        CreateMap<UpdateFreelancerSkillCommand, FreelancerSkill>()
            .ForMember(dest => dest.NormalizedName, opt
                => opt.MapFrom(src => src.Name.ToUpperInvariant().Replace(' ', '_')));
    }
}

using IdentityService.BLL.UseCases.UserUseCases.Commands.RegisterFreelancer;

namespace IdentityService.BLL.Mapping.UserMappingProfiles;

public class RegisterFreelancerCommandToFreelancerProfileProfile : Profile
{
    public RegisterFreelancerCommandToFreelancerProfileProfile()
    {
        CreateMap<RegisterFreelancerCommand, FreelancerProfile>()
            .ForMember(dest => dest.Skills, opt =>
                opt.MapFrom(_ => new List<FreelancerSkill>()))
            .ForMember(dest => dest.About, opt =>
                opt.MapFrom(_ => string.Empty));
    }
}
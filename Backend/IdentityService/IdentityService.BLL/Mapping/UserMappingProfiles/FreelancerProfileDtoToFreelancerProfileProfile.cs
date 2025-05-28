using IdentityService.BLL.DTOs;

namespace IdentityService.BLL.Mapping.UserMappingProfiles;

public class FreelancerProfileDtoToFreelancerProfileProfile : Profile
{
    public FreelancerProfileDtoToFreelancerProfileProfile()
    {
        CreateMap<FreelancerProfileDto, FreelancerProfile>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.Skills, opt => opt.Ignore());
    }
}
using IdentityService.BLL.DTOs;

namespace IdentityService.BLL.Mapping.UserMappingProfiles;

public class FreelancerProfileDTOToFreelancerProfileProfile : Profile
{
    public FreelancerProfileDTOToFreelancerProfileProfile()
    {
        CreateMap<FreelancerProfileDTO, FreelancerProfile>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore());
    }
}

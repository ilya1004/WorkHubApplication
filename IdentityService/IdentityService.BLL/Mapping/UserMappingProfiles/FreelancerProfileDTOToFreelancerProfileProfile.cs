using IdentityService.BLL.DTOs;

namespace IdentityService.BLL.Mapping.UserMappingProfiles;

public class FreelancerProfileDTOToFreelancerProfileProfile : Profile
{
    public FreelancerProfileDTOToFreelancerProfileProfile()
    {
        CreateMap<FreelancerProfileDTO, FreelancerProfile>();
    }
}

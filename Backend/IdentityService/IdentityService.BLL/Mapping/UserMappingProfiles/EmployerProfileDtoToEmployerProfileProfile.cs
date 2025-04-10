using IdentityService.BLL.DTOs;

namespace IdentityService.BLL.Mapping.UserMappingProfiles;

public class EmployerProfileDtoToEmployerProfileProfile : Profile
{
    public EmployerProfileDtoToEmployerProfileProfile()
    {
        CreateMap<EmployerProfileDto, EmployerProfile>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.Industry, opt => opt.Ignore());
    }
}
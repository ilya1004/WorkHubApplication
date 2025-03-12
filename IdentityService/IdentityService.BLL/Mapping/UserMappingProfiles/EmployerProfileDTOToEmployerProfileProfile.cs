using IdentityService.BLL.DTOs;

namespace IdentityService.BLL.Mapping.UserMappingProfiles;

public class EmployerProfileDTOToEmployerProfileProfile : Profile
{
    public EmployerProfileDTOToEmployerProfileProfile()
    {
        CreateMap<EmployerProfileDto, EmployerProfile>();
    }
}

using IdentityService.BLL.DTOs;

namespace IdentityService.BLL.Mapping.UserMappingProfiles;

public class EmployerProfileDtoToEmployerProfileProfile : Profile
{
    public EmployerProfileDtoToEmployerProfileProfile()
    {
        CreateMap<EmployerProfileDto, EmployerProfile>();
    }
}
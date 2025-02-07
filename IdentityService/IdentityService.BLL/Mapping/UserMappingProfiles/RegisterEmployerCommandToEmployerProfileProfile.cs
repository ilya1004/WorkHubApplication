using IdentityService.BLL.UseCases.UserUseCases.Commands.RegisterEmployer;
using IdentityService.BLL.UseCases.UserUseCases.Commands.RegisterFreelancer;

namespace IdentityService.BLL.Mapping.UserMappingProfiles;

public class RegisterEmployerCommandToEmployerProfileProfile : Profile
{
    public RegisterEmployerCommandToEmployerProfileProfile()
    {
        CreateMap<RegisterEmployerCommand, EmployerProfile>()
            .ForMember(dest => dest.About, opt =>
                opt.MapFrom(_ => string.Empty));
    }
}

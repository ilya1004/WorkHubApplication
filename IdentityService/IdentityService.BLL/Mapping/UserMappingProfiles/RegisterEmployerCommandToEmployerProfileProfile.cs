using IdentityService.BLL.UseCases.UserUseCases.Commands.RegisterEmployer;

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
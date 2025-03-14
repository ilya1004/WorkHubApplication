using IdentityService.BLL.UseCases.UserUseCases.Commands.RegisterEmployer;

namespace IdentityService.BLL.Mapping.UserMappingProfiles;

public class RegisterEmployerCommandToAppUserProfile : Profile
{
    public RegisterEmployerCommandToAppUserProfile()
    {
        CreateMap<RegisterEmployerCommand, AppUser>()
            .ForMember(dest => dest.EmailConfirmed, opt =>
                opt.MapFrom(_ => false))
            .ForMember(dest => dest.RegisteredAt, opt =>
                opt.MapFrom(_ => DateTime.UtcNow));
    }
}
using IdentityService.BLL.UseCases.UserUseCases.Commands.RegisterFreelancer;

namespace IdentityService.BLL.Mapping.UserMappingProfiles;

public class RegisterFreelancerCommandToAppUserProfile : Profile
{
    public RegisterFreelancerCommandToAppUserProfile()
    {
        CreateMap<RegisterFreelancerCommand, AppUser>()
            .ForMember(dest => dest.EmailConfirmed, opt =>
                opt.MapFrom(_ => false))
            .ForMember(dest => dest.RegisteredAt, opt =>
                opt.MapFrom(_ => DateTime.UtcNow));
    }
}
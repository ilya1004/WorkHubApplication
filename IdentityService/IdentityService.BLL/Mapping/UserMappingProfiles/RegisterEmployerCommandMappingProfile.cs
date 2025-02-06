using AutoMapper;
using IdentityService.BLL.UseCases.UserUseCases.Commands.RegisterEmployer;
using IdentityService.DAL.Entities;

namespace IdentityService.BLL.Mapping.UserMappingProfiles;

public class RegisterEmployerCommandMappingProfile : Profile
{
    public RegisterEmployerCommandMappingProfile()
    {
        CreateMap<RegisterEmployerCommand, AppUser>()
            .ForMember(dest => dest.UserName, opt =>
                opt.MapFrom(src => src.CompanyName))
            .ForMember(dest => dest.Email, opt =>
                opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.EmailConfirmed, opt =>
                opt.MapFrom(_ => false))
            .ForMember(dest => dest.RegisteredAt, opt =>
                opt.MapFrom(_ => DateTime.UtcNow));
    }
}

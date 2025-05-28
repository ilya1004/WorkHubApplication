using IdentityService.BLL.DTOs;

namespace IdentityService.BLL.Mapping.UserMappingProfiles;

public class AppUserToEmployerUserDto : Profile
{
    public AppUserToEmployerUserDto()
    {
        CreateMap<AppUser, EmployerUserDto>()
            .ForMember(dest => dest.Id, opt =>
                opt.MapFrom(src => src.Id.ToString()))
            .ForMember(dest => dest.UserName, opt =>
                opt.MapFrom(src => src.UserName))
            .ForMember(dest => dest.CompanyName, opt =>
                opt.MapFrom(src => src.EmployerProfile!.CompanyName))
            .ForMember(dest => dest.About, opt =>
                opt.MapFrom(src => src.EmployerProfile!.About))
            .ForMember(dest => dest.Email, opt =>
                opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.RegisteredAt, opt =>
                opt.MapFrom(src => src.RegisteredAt))
            .ForMember(dest => dest.StripeCustomerId, opt =>
                opt.MapFrom(src => src.EmployerProfile!.StripeCustomerId))
            .ForMember(dest => dest.Industry, opt =>
                opt.MapFrom(src => new EmployerIndustryDto(src.EmployerProfile!.Industry!.Id.ToString(), src.EmployerProfile.Industry.Name)))
            .ForMember(dest => dest.ImageUrl, opt =>
                opt.MapFrom(src => src.ImageUrl))
            .ForMember(dest => dest.RoleName, opt =>
                opt.MapFrom(src => src.Role.Name));
    }
}
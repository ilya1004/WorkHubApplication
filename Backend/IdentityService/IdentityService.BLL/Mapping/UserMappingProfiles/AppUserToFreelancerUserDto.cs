using IdentityService.BLL.DTOs;

namespace IdentityService.BLL.Mapping.UserMappingProfiles;

public class AppUserToFreelancerUserDto : Profile
{
    public AppUserToFreelancerUserDto()
    {
        CreateMap<AppUser, FreelancerUserDto>()
            .ForMember(dest => dest.Id, opt =>
                opt.MapFrom(src => src.Id.ToString()))
            .ForMember(dest => dest.UserName, opt =>
                opt.MapFrom(src => src.UserName))
            .ForMember(dest => dest.FirstName, opt =>
                opt.MapFrom(src => src.FreelancerProfile!.FirstName))
            .ForMember(dest => dest.LastName, opt =>
                opt.MapFrom(src => src.FreelancerProfile!.LastName))
            .ForMember(dest => dest.About, opt =>
                opt.MapFrom(src => src.FreelancerProfile!.About))
            .ForMember(dest => dest.Email, opt =>
                opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.RegisteredAt, opt =>
                opt.MapFrom(src => src.RegisteredAt))
            .ForMember(dest => dest.StripeAccountId, opt =>
                opt.MapFrom(src => src.FreelancerProfile!.StripeAccountId))
            .ForMember(dest => dest.Skills, opt =>
                opt.MapFrom(src => src.FreelancerProfile!.Skills.Select(s => 
                    new FreelancerSkillDto(s.Id.ToString(), s.Name)).ToList()))
            .ForMember(dest => dest.ImageUrl, opt =>
                opt.MapFrom(src => src.ImageUrl))
            .ForMember(dest => dest.RoleName, opt =>
                opt.MapFrom(src => src.Role.Name));
    }
}
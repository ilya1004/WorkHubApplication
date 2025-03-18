using IdentityService.BLL.UseCases.EmployerIndustryUseCases.Commands.UpdateEmployerIndustry;

namespace IdentityService.BLL.Mapping.EmployerIndustryMappingProfiles;

public class UpdateEmployerIndustryCommandToEmployerIndustryProfile : Profile
{
    public UpdateEmployerIndustryCommandToEmployerIndustryProfile()
    {
        CreateMap<UpdateEmployerIndustryCommand, EmployerIndustry>()
            .ForMember(dest => dest.NormalizedName, opt
                => opt.MapFrom(src => src.Name.ToUpperInvariant().Replace(' ', '_')));
    }
}
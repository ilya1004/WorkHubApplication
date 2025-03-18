using IdentityService.BLL.UseCases.EmployerIndustryUseCases.Commands.CreateEmployerIndustry;

namespace IdentityService.BLL.Mapping.EmployerIndustryMappingProfiles;

public class CreateEmployerIndustryCommandToEmployerIndustryProfile : Profile
{
    public CreateEmployerIndustryCommandToEmployerIndustryProfile()
    {
        CreateMap<CreateEmployerIndustryCommand, EmployerIndustry>()
            .ForMember(dest => dest.NormalizedName, opt
                => opt.MapFrom(src => src.Name.ToUpperInvariant().Replace(' ', '_')));
    }
}
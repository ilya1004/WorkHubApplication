using ProjectsService.Application.UseCases.Commands.CategoryUseCases.UpdateCategory;

namespace ProjectsService.Application.Mapping.CategoryMappingProfiles;

public class UpdateCategoryCommandToCategory : Profile
{
    public UpdateCategoryCommandToCategory()
    {
        CreateMap<UpdateCategoryCommand, Category>()
            .ForMember(dest => dest.NormalizedName, opt 
                => opt.MapFrom(src => src.Name.ToUpperInvariant().Replace(' ', '_')));
    }
}
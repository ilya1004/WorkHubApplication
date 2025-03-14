using ProjectsService.Application.UseCases.Commands.CategoryUseCases.CreateCategory;

namespace ProjectsService.Application.Mapping.CategoryMappingProfiles;

public class CreateCategoryCommandToCategory : Profile
{
    public CreateCategoryCommandToCategory()
    {
        CreateMap<CreateCategoryCommand, Category>()
            .ForMember(dest => dest.NormalizedName, opt =>
                opt.MapFrom(src => src.Name.ToUpperInvariant().Replace(' ', '_')));
    }
}
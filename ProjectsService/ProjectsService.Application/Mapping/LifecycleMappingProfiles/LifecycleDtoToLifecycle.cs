using ProjectsService.Domain.Enums;

namespace ProjectsService.Application.Mapping.LifecycleMappingProfiles;

public class LifecycleDtoToLifecycle : Profile
{
    public LifecycleDtoToLifecycle()
    {
        CreateMap<LifecycleDto, Lifecycle>()
            .ForMember(dest => dest.Id, opt =>
                opt.MapFrom(_ => Guid.NewGuid()))
            .ForMember(dest => dest.CreatedAt, opt => 
                opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => 
                opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.Status, opt => 
                opt.MapFrom(_ => ProjectStatus.Published));
    }
}
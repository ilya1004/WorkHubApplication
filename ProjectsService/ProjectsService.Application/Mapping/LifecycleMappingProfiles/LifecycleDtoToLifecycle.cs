namespace ProjectsService.Application.Mapping.LifecycleMappingProfiles;

public class LifecycleDtoToLifecycle : Profile
{
    public LifecycleDtoToLifecycle()
    {
        CreateMap<LifecycleDto, Lifecycle>()
            .ForMember(dest => dest.UpdatedAt, opt => 
                opt.MapFrom(src => DateTime.UtcNow));
    }
}
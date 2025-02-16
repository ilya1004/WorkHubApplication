namespace ProjectsService.Application.Mapping.LifecycleMappingProfiles;

public class LifecycleDtoToLifecycle : Profile
{
    public LifecycleDtoToLifecycle()
    {
        CreateMap<LifecycleDto, Lifecycle>();
    }
}
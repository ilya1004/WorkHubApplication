namespace ProjectsService.Application.Mapping.ProjectMappingProfiles;

public class ProjectDtoToProject : Profile
{
    public ProjectDtoToProject()
    {
        CreateMap<ProjectDto, Project>()
            .ForMember(dest => dest.Id, opt =>
                opt.MapFrom(_ => Guid.NewGuid()));
    }
}
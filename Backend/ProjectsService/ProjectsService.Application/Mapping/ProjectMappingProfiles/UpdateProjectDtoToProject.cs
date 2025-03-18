namespace ProjectsService.Application.Mapping.ProjectMappingProfiles;

public class UpdateProjectDtoToProject : Profile
{
    public UpdateProjectDtoToProject()
    {
        CreateMap<UpdateProjectDto, Project>();
    }
}
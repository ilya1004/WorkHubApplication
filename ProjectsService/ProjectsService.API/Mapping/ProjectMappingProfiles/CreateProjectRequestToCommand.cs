using ProjectsService.API.Contracts.ProjectContracts;
using ProjectsService.Application.UseCases.Commands.ProjectUseCases.CreateProject;

namespace ProjectsService.API.Mapping.ProjectMappingProfiles;

public class CreateProjectRequestToCommand : Profile
{
    public CreateProjectRequestToCommand()
    {
        CreateMap<CreateProjectRequest, CreateProjectCommand>();
    }
}
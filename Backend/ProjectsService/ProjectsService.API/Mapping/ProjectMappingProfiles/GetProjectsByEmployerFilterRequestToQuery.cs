using ProjectsService.API.Contracts.ProjectContracts;
using ProjectsService.Application.UseCases.Queries.ProjectUseCases.GetProjectsByEmployerFilter;

namespace ProjectsService.API.Mapping.ProjectMappingProfiles;

public class GetProjectsByEmployerFilterRequestToQuery : Profile
{
    public GetProjectsByEmployerFilterRequestToQuery()
    {
        CreateMap<GetProjectsByEmployerFilterRequest, GetProjectsByEmployerFilterQuery>();
    }
}
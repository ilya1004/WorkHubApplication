using ProjectsService.API.Contracts.ProjectContracts;
using ProjectsService.Application.UseCases.Queries.ProjectUseCases.GetProjectsByFilter;

namespace ProjectsService.API.Mapping.ProjectMappingProfiles;

public class GetProjectsByFilterRequestToQuery : Profile
{
    public GetProjectsByFilterRequestToQuery()
    {
        CreateMap<GetProjectsByFilterRequest, GetProjectsByFilterQuery>();
    }
}
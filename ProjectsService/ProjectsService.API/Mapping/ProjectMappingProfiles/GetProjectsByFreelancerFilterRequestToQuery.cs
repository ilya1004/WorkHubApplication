using ProjectsService.API.Contracts.ProjectContracts;
using ProjectsService.Application.UseCases.Queries.ProjectUseCases.GetProjectsByFreelancerFilter;

namespace ProjectsService.API.Mapping.ProjectMappingProfiles;

public class GetProjectsByFreelancerFilterRequestToQuery : Profile
{
    public GetProjectsByFreelancerFilterRequestToQuery()
    {
        CreateMap<GetProjectsByFreelancerFilterRequest, GetProjectsByFreelancerFilterQuery>();
    }
}
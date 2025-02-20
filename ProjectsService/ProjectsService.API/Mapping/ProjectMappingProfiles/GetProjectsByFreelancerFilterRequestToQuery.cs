using ProjectsService.API.Contracts.ProjectContracts;
using ProjectsService.Application.UseCases.Queries.ProjectUseCases.GetProjectsByFreelancerFilter;

namespace ProjectsService.API.Mapping.ProjectMappingProfiles;

public class GetProjectsByFreelancerFilterRequestToQuery : Profile
{
    public GetProjectsByFreelancerFilterRequestToQuery()
    {
        CreateMap<GetProjectsByFreelancerFilterRequest, GetProjectsByFreelancerFilterQuery>()
            .ConstructUsing(src => new GetProjectsByFreelancerFilterQuery(
                Guid.Empty, src.ProjectStatus, src.EmployerId, src.PageNo, src.PageSize));
    }
}
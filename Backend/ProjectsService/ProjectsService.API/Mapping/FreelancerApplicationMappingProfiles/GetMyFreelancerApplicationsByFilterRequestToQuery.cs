using ProjectsService.API.Contracts.FreelancerApplicationContracts;
using ProjectsService.Application.UseCases.Queries.FreelancerApplicationUseCases.GetMyFreelancerApplicationsByFilter;

namespace ProjectsService.API.Mapping.FreelancerApplicationMappingProfiles;

public class GetMyFreelancerApplicationsByFilterRequestToQuery : Profile
{
    public GetMyFreelancerApplicationsByFilterRequestToQuery()
    {
        CreateMap<GetMyFreelancerApplicationsByFilterRequest, GetMyFreelancerApplicationsByFilterQuery>();
    }
}
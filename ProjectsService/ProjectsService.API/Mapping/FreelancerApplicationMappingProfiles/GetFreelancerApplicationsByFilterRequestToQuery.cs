using ProjectsService.API.Contracts.FreelancerApplicationContracts;
using ProjectsService.Application.UseCases.Queries.FreelancerApplicationUseCases.GetFreelancerApplicationsByFilter;

namespace ProjectsService.API.Mapping.FreelancerApplicationMappingProfiles;

public class GetFreelancerApplicationsByFilterRequestToQuery : Profile
{
    public GetFreelancerApplicationsByFilterRequestToQuery()
    {
        CreateMap<GetFreelancerApplicationsByFilterRequest, GetFreelancerApplicationsByFilterQuery>()
            .ConstructUsing(src => new GetFreelancerApplicationsByFilterQuery(
                Guid.Empty, src.StartDate, src.EndDate, src.ApplicationStatus, src.PageNo, src.PageSize));
    }
}
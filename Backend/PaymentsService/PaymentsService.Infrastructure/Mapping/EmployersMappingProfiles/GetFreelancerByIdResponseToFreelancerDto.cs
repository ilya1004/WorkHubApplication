using Freelancers;

namespace PaymentsService.Infrastructure.Mapping.EmployersMappingProfiles;

public class GetFreelancerByIdResponseToFreelancerDto : Profile
{
    public GetFreelancerByIdResponseToFreelancerDto()
    {
        CreateMap<GetFreelancerByIdResponse, FreelancerDto>();
    }
}
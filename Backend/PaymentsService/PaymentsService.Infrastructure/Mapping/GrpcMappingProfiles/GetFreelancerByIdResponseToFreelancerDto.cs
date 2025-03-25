using Freelancers;

namespace PaymentsService.Infrastructure.Mapping.GrpcMappingProfiles;

public class GetFreelancerByIdResponseToFreelancerDto : Profile
{
    public GetFreelancerByIdResponseToFreelancerDto()
    {
        CreateMap<GetFreelancerByIdResponse, FreelancerDto>();
    }
}
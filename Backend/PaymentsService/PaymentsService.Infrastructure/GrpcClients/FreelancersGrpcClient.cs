using Freelancers;
using PaymentsService.Infrastructure.Interfaces;

namespace PaymentsService.Infrastructure.GrpcClients;

public class FreelancersGrpcClient(IMapper mapper, Freelancers.Freelancers.FreelancersClient client) : IFreelancersGrpcClient
{
    public async Task<FreelancerDto> GetFreelancerByIdAsync(string id, CancellationToken cancellationToken)
    {
        var response = await client.GetFreelancerByIdAsync(new GetFreelancerByIdRequest { Id = id }, cancellationToken: cancellationToken);
        
        var freelancerDto = mapper.Map<FreelancerDto>(response);

        return freelancerDto;
    }
}
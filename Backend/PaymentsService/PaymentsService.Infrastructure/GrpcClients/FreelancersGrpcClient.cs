using Freelancers;
using Microsoft.Extensions.Logging;
using PaymentsService.Infrastructure.Interfaces;

namespace PaymentsService.Infrastructure.GrpcClients;

public class FreelancersGrpcClient(
    IMapper mapper, 
    Freelancers.Freelancers.FreelancersClient client,
    ILogger<FreelancersGrpcClient> logger) : IFreelancersGrpcClient
{
    public async Task<FreelancerDto> GetFreelancerByIdAsync(string id, CancellationToken cancellationToken)
    {
        logger.LogInformation("Requesting freelancer with ID {FreelancerId} from gRPC service", id);
        
        var response = await client.GetFreelancerByIdAsync(
            new GetFreelancerByIdRequest { Id = id }, 
            cancellationToken: cancellationToken);
        
        logger.LogInformation("Successfully received freelancer with ID {FreelancerId} from gRPC service", id);

        var freelancerDto = mapper.Map<FreelancerDto>(response);

        return freelancerDto;
    }
}
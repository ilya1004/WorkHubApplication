using Employers;
using PaymentsService.Infrastructure.Interfaces;

namespace PaymentsService.Infrastructure.GrpcClients;

public class EmployersGrpcClient(
    IMapper mapper, 
    Employers.Employers.EmployersClient client,
    ILogger<EmployersGrpcClient> logger) : IEmployersGrpcClient
{
    public async Task<EmployerDto> GetEmployerByIdAsync(string id, CancellationToken cancellationToken)
    {
        logger.LogInformation("Requesting employer with ID {EmployerId} from gRPC service", id);
        
        var response = await client.GetEmployerByIdAsync(
            new GetEmployerByIdRequest { Id = id }, 
            cancellationToken: cancellationToken);
        
        if (response is null)
        {
            logger.LogWarning("Employer not found for user {UserId}", id);
            
            throw new NotFoundException($"Employer by user ID '{id}' not found.");
        }
        
        logger.LogInformation("Successfully received employer with ID {EmployerId} from gRPC service", id);

        var employerDto = mapper.Map<EmployerDto>(response);

        return employerDto;
    }
}

using Employers;
using Microsoft.Extensions.Logging;
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
        
        logger.LogInformation("Successfully received employer with ID {EmployerId} from gRPC service", id);

        var employerDto = mapper.Map<EmployerDto>(response);
        
        logger.LogDebug("Mapped gRPC response to EmployerDto for ID {EmployerId}", id);

        return employerDto;
    }
}

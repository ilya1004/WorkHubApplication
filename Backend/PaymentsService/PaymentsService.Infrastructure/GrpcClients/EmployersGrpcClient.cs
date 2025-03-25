using Employers;
using PaymentsService.Infrastructure.Interfaces;


namespace PaymentsService.Infrastructure.GrpcClients;

public class EmployersGrpcClient(IMapper mapper, Employers.Employers.EmployersClient client) : IEmployersGrpcClient
{
    public async Task<EmployerDto> GetEmployerByIdAsync(string id, CancellationToken cancellationToken)
    {
        var response = await client.GetEmployerByIdAsync(new GetEmployerByIdRequest { Id = id }, cancellationToken: cancellationToken);
        
        var employerDto = mapper.Map<EmployerDto>(response);

        return employerDto;
    }
}
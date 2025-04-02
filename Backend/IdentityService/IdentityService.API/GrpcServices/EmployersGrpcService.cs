using Employers;
using Grpc.Core;
using IdentityService.BLL.UseCases.UserUseCases.Queries.GetUserById;

namespace IdentityService.API.GrpcServices;

public class EmployersGrpcService(
    IMediator mediator,
    ILogger<EmployersGrpcService> logger) : Employers.Employers.EmployersBase
{
    [Authorize]
    public override async Task<GetEmployerByIdResponse> GetEmployerById(GetEmployerByIdRequest request, ServerCallContext context)
    {
        logger.LogInformation("Getting employer for ID: {EmployerId}", request.Id);
        
        var appUser = await mediator.Send(new GetUserByIdQuery(Guid.Parse(request.Id)));
        
        logger.LogInformation("Successfully returned employer data for {EmployerId}", request.Id);
        
        return new GetEmployerByIdResponse
        {
            Id = appUser.Id.ToString(), 
            EmployerCustomerId = appUser.EmployerProfile?.StripeCustomerId ?? string.Empty,
        };
    }
}
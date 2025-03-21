using Employers;
using Grpc.Core;
using IdentityService.BLL.UseCases.UserUseCases.Queries.GetUserById;

namespace IdentityService.API.GrpcServices;

public class EmployersGrpcService(IMediator mediator) : Employers.Employers.EmployersBase
{
    [Authorize]
    public override async Task<GetEmployerByIdResponse> GetEmployerById(GetEmployerByIdRequest request, ServerCallContext context)
    {
        var appUser = await mediator.Send(new GetUserByIdQuery(Guid.Parse(request.Id)));
        
        return new GetEmployerByIdResponse
        {
            Id = appUser.Id.ToString(), 
            EmployerCustomerId = appUser.EmployerProfile?.StripeCustomerId ?? string.Empty,
        };
    }
}
using Freelancers;
using Grpc.Core;
using IdentityService.BLL.UseCases.UserUseCases.Queries.GetUserById;

namespace IdentityService.API.GrpcServices;

public class FreelancersGrpcService(IMediator mediator) : Freelancers.Freelancers.FreelancersBase
{
    [Authorize]
    public override async Task<GetFreelancerByIdResponse> GetFreelancerById(GetFreelancerByIdRequest request, ServerCallContext context)
    {
        var appUser = await mediator.Send(new GetUserByIdQuery(Guid.Parse(request.Id)));

        return new GetFreelancerByIdResponse
        {
            Id = appUser.Id.ToString(), 
            StripeAccountId = appUser.FreelancerProfile?.StripeAccountId ?? string.Empty
        };
    }
}
using Freelancers;
using Grpc.Core;
using IdentityService.BLL.UseCases.UserUseCases.Queries.GetUserById;

namespace IdentityService.API.GrpcServices;

public class FreelancersGrpcService(
    IMediator mediator, 
    ILogger<FreelancersGrpcService> logger) : Freelancers.Freelancers.FreelancersBase
{
    [Authorize]
    public override async Task<GetFreelancerByIdResponse> GetFreelancerById(GetFreelancerByIdRequest request, ServerCallContext context)
    {
        logger.LogInformation("Getting freelancer for ID: {FreelancerId}", request.Id);
        
        var appUser = await mediator.Send(new GetUserByIdQuery(Guid.Parse(request.Id)));
        
        logger.LogInformation("Successfully returned freelancer data for {FreelancerId}", request.Id);

        return new GetFreelancerByIdResponse
        {
            Id = appUser.Id.ToString(), 
            StripeAccountId = appUser.FreelancerProfile?.StripeAccountId ?? string.Empty
        };
    }
}
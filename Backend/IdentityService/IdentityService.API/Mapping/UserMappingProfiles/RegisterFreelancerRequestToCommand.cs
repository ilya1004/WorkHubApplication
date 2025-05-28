using IdentityService.API.Contracts.UserContracts;
using IdentityService.BLL.UseCases.UserUseCases.Commands.RegisterFreelancer;

namespace IdentityService.API.Mapping.UserMappingProfiles;

public class RegisterFreelancerRequestToCommand : Profile
{
    public RegisterFreelancerRequestToCommand()
    {
        CreateMap<RegisterFreelancerRequest, RegisterFreelancerCommand>();
    }
}
using AutoMapper;
using IdentityService.API.Contracts;
using IdentityService.BLL.UseCases.UserUseCases.Commands.RegisterFreelancer;

namespace IdentityService.API.Mapping.UserMappingProfiles;

public class RegisterFreelancerRequestToCommandProfile : Profile
{
    public RegisterFreelancerRequestToCommandProfile()
    {
        CreateMap<RegisterFreelancerRequest, RegisterFreelancerCommand>();
    }
}

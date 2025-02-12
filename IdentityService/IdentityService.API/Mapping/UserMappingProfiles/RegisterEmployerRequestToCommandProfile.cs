using IdentityService.API.Contracts.UserContracts;
using IdentityService.BLL.UseCases.UserUseCases.Commands.RegisterEmployer;

namespace IdentityService.API.Mapping.UserMappingProfiles;

public class RegisterEmployerRequestToCommandProfile : Profile
{
    public RegisterEmployerRequestToCommandProfile()
    {
        CreateMap<RegisterEmployerRequest, RegisterEmployerCommand>();
    }
}

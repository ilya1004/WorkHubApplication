using IdentityService.API.Contracts.UserContracts;
using IdentityService.BLL.UseCases.UserUseCases.Commands.RegisterEmployer;

namespace IdentityService.API.Mapping.UserMappingProfiles;

public class RegisterEmployerRequestToCommand : Profile
{
    public RegisterEmployerRequestToCommand()
    {
        CreateMap<RegisterEmployerRequest, RegisterEmployerCommand>();
    }
}
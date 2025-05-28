using IdentityService.API.Contracts.UserContracts;
using IdentityService.BLL.UseCases.UserUseCases.Commands.ChangePassword;

namespace IdentityService.API.Mapping.UserMappingProfiles;

public class ChangePasswordRequestToCommand : Profile
{
    public ChangePasswordRequestToCommand()
    {
        CreateMap<ChangePasswordRequest, ChangePasswordCommand>();
    }
}
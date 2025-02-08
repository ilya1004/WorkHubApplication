using IdentityService.API.Contracts.UserContracts;
using IdentityService.BLL.UseCases.UserUseCases.Commands.ChangePassword;

namespace IdentityService.API.Mapping.UserMappingProfiles;

public class ChangePasswordRequestToCommandProfile : Profile
{
    public ChangePasswordRequestToCommandProfile()
    {
        CreateMap<ChangePasswordRequest, ChangePasswordCommand>();
    }
}

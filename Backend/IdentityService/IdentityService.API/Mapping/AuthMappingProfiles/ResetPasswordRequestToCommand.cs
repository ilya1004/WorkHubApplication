using IdentityService.API.Contracts.AuthContracts;
using IdentityService.BLL.UseCases.AuthUseCases.ResetPassword;

namespace IdentityService.API.Mapping.AuthMappingProfiles;

public class ResetPasswordRequestToCommand : Profile
{
    public ResetPasswordRequestToCommand()
    {
        CreateMap<ResetPasswordRequest, ResetPasswordCommand>();
    }
}
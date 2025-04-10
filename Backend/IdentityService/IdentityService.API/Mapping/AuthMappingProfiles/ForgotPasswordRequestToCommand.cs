using IdentityService.API.Contracts.AuthContracts;
using IdentityService.BLL.UseCases.AuthUseCases.ForgotPassword;

namespace IdentityService.API.Mapping.AuthMappingProfiles;

public class ForgotPasswordRequestToCommand : Profile
{
    public ForgotPasswordRequestToCommand()
    {
        CreateMap<ForgotPasswordRequest, ForgotPasswordCommand>();
    }
}
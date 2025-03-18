using IdentityService.API.Contracts.AuthContracts;
using IdentityService.BLL.UseCases.AuthUseCases.ForgotPassword;

namespace IdentityService.API.Mapping.AuthMappingProfiles;

public class ForgotPasswordRequestToCommandProfile : Profile
{
    public ForgotPasswordRequestToCommandProfile()
    {
        CreateMap<ForgotPasswordRequest, ForgotPasswordCommand>();
    }
}
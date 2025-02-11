using IdentityService.API.Contracts.AuthContracts;
using IdentityService.BLL.UseCases.AuthUseCases.ResetPassword;

namespace IdentityService.API.Mapping.AuthMappingProfiles;

public class ResetPasswordRequestToCommandProfile : Profile
{
    public ResetPasswordRequestToCommandProfile()
    {
        CreateMap<ResetPasswordRequest, ResetPasswordCommand>();
    }
}

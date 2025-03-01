using IdentityService.API.Contracts.AuthContracts;
using IdentityService.BLL.UseCases.AuthUseCases.LoginUser;

namespace IdentityService.API.Mapping.AuthMappingProfiles;

public class LoginUserRequestToCommandProfile : Profile
{
    public LoginUserRequestToCommandProfile()
    {
        CreateMap<LoginUserRequest, LoginUserCommand>();
    }
}

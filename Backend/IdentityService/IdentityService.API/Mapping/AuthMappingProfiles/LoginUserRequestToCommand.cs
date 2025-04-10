using IdentityService.API.Contracts.AuthContracts;
using IdentityService.BLL.UseCases.AuthUseCases.LoginUser;

namespace IdentityService.API.Mapping.AuthMappingProfiles;

public class LoginUserRequestToCommand : Profile
{
    public LoginUserRequestToCommand()
    {
        CreateMap<LoginUserRequest, LoginUserCommand>();
    }
}
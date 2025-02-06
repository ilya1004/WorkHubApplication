using AutoMapper;
using IdentityService.API.Contracts;
using IdentityService.BLL.UseCases.AuthUseCases.LoginUser;

namespace IdentityService.API.Mapping.UserMappingProfiles;

public class LoginUserRequestToCommandProfile : Profile
{
    public LoginUserRequestToCommandProfile()
    {
        CreateMap<LoginUserRequest, LoginUserCommand>();
    }
}

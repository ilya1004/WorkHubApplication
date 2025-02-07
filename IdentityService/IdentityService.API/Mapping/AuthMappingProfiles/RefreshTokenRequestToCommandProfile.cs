using IdentityService.API.Contracts;
using IdentityService.BLL.UseCases.AuthUseCases.RefreshToken;

namespace IdentityService.API.Mapping.AuthMappingProfiles;

public class RefreshTokenRequestToCommandProfile : Profile
{
    public RefreshTokenRequestToCommandProfile()
    {
        CreateMap<RefreshTokenRequest, RefreshTokenCommand>();
    }
}

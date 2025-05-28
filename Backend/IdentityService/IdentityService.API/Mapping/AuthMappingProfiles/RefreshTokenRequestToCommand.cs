using IdentityService.API.Contracts.AuthContracts;
using IdentityService.BLL.UseCases.AuthUseCases.RefreshToken;

namespace IdentityService.API.Mapping.AuthMappingProfiles;

public class RefreshTokenRequestToCommand : Profile
{
    public RefreshTokenRequestToCommand()
    {
        CreateMap<RefreshTokenRequest, RefreshTokenCommand>();
    }
}
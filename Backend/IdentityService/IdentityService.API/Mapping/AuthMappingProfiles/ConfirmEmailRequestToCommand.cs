using IdentityService.API.Contracts.AuthContracts;
using IdentityService.BLL.UseCases.AuthUseCases.ConfirmEmail;

namespace IdentityService.API.Mapping.AuthMappingProfiles;

public class ConfirmEmailRequestToCommand : Profile
{
    public ConfirmEmailRequestToCommand()
    {
        CreateMap<ConfirmEmailRequest, ConfirmEmailCommand>();
    }
}
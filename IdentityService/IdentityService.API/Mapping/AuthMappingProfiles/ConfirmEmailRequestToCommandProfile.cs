using IdentityService.API.Contracts.AuthContracts;
using IdentityService.BLL.UseCases.AuthUseCases.ConfirmEmail;

namespace IdentityService.API.Mapping.AuthMappingProfiles;

public class ConfirmEmailRequestToCommandProfile : Profile
{
    public ConfirmEmailRequestToCommandProfile()
    {
        CreateMap<ConfirmEmailRequest, ConfirmEmailCommand>();
    }
}

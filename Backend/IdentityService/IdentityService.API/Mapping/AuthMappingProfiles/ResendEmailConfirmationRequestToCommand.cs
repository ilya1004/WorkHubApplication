using IdentityService.API.Contracts.AuthContracts;
using IdentityService.BLL.UseCases.AuthUseCases.ResendEmailConfirmation;

namespace IdentityService.API.Mapping.AuthMappingProfiles;

public class ResendEmailConfirmationRequestToCommand : Profile
{
    public ResendEmailConfirmationRequestToCommand()
    {
        CreateMap<ResendEmailConfirmationRequest, ResendEmailConfirmationCommand>();
    }
}
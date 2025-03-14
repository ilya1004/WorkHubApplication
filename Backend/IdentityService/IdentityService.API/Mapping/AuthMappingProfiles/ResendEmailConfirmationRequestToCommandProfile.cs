using IdentityService.API.Contracts.AuthContracts;
using IdentityService.BLL.UseCases.AuthUseCases.ResendEmailConfirmation;

namespace IdentityService.API.Mapping.AuthMappingProfiles;

public class ResendEmailConfirmationRequestToCommandProfile : Profile
{
    public ResendEmailConfirmationRequestToCommandProfile()
    {
        CreateMap<ResendEmailConfirmationRequest, ResendEmailConfirmationCommand>();
    }
}
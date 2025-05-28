using IdentityService.API.Contracts.UserContracts;
using IdentityService.BLL.UseCases.UserUseCases.Commands.UpdateEmployerProfile;

namespace IdentityService.API.Mapping.UserMappingProfiles;

public class UpdateEmployerProfileRequestToCommand : Profile
{
    public UpdateEmployerProfileRequestToCommand()
    {
        CreateMap<UpdateEmployerProfileRequest, UpdateEmployerProfileCommand>()
            .ConstructUsing(src =>
                new UpdateEmployerProfileCommand(
                    src.EmployerProfile,
                    src.ImageFile == null ? null : src.ImageFile.OpenReadStream(), 
                    src.ImageFile == null ? null : src.ImageFile.ContentType));
    }
}
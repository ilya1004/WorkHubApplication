using IdentityService.API.Contracts.UserContracts;
using IdentityService.BLL.UseCases.UserUseCases.Commands.UpdateFreelancerProfile;

namespace IdentityService.API.Mapping.UserMappingProfiles;

public class UpdateFreelancerProfileRequestToCommand : Profile
{
    public UpdateFreelancerProfileRequestToCommand()
    {
        CreateMap<UpdateFreelancerProfileRequest, UpdateFreelancerProfileCommand>()
            .ConstructUsing(src =>
                new UpdateFreelancerProfileCommand(
                    src.FreelancerProfile,
                    src.ImageFile == null ? null : src.ImageFile.OpenReadStream(), 
                    src.ImageFile == null ? null : src.ImageFile.ContentType));
    }
}
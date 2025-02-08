using IdentityService.API.Contracts.UserContracts;
using IdentityService.BLL.UseCases.UserUseCases.Commands.UpdateFreelancerProfile;

namespace IdentityService.API.Mapping.UserMappingProfiles;

public class UpdateFreelancerProfileRequestToCommand : Profile
{
    public UpdateFreelancerProfileRequestToCommand()
    {
        CreateMap<UpdateFreelancerProfileRequest, UpdateFreelancerProfileCommand>()
            .ForMember(dest => dest.FileStream, opt => 
                opt.MapFrom(src => src.ImageFile == null ? null : src.ImageFile.OpenReadStream()))
            .ForMember(dest => dest.ContentType, opt =>
                opt.MapFrom(src => src.ImageFile == null ? null : src.ImageFile.ContentType));
    }
}
// TODELETE
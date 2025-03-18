using IdentityService.API.Contracts.UserContracts;
using IdentityService.BLL.UseCases.UserUseCases.Commands.UpdateEmployerProfile;

namespace IdentityService.API.Mapping.UserMappingProfiles;

public class UpdateEmployerProfileRequestToCommand : Profile
{
    public UpdateEmployerProfileRequestToCommand()
    {
        CreateMap<UpdateEmployerProfileRequest, UpdateEmployerProfileCommand>()
            .ForMember(dest => dest.EmployerProfile, opt =>
                opt.MapFrom(src => src.EmployerProfile))
            .ForMember(dest => dest.FileStream, opt =>
                opt.MapFrom(src => src.ImageFile == null ? null : src.ImageFile.OpenReadStream()))
            .ForMember(dest => dest.ContentType, opt =>
                opt.MapFrom(src => src.ImageFile == null ? null : src.ImageFile.ContentType));
    }
}
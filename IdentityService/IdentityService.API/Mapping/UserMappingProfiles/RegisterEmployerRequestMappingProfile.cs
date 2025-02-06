using AutoMapper;
using IdentityService.API.Contracts;
using IdentityService.BLL.UseCases.UserUseCases.Commands.RegisterEmployer;

namespace IdentityService.API.Mapping.UserMappingProfiles;

public class RegisterEmployerRequestMappingProfile : Profile
{
    public RegisterEmployerRequestMappingProfile()
    {
        CreateMap<RegisterEmployerRequest, RegisterEmployerCommand>();
    }
}

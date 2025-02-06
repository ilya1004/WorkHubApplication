using AutoMapper;
using IdentityService.API.Contracts;
using IdentityService.BLL.UseCases.UserUseCases.Commands.RegisterEmployer;

namespace IdentityService.API.Mapping.UserMappingProfiles;

public class RegisterEmployerRequestToCommandProfile : Profile
{
    public RegisterEmployerRequestToCommandProfile()
    {
        CreateMap<RegisterEmployerRequest, RegisterEmployerCommand>();
    }
}

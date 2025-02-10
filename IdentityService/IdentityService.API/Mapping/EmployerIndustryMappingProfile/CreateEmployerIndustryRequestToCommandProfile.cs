using IdentityService.API.Contracts.EmployerIndustryContracts;
using IdentityService.BLL.UseCases.EmployerIndustryUseCases.Commands.CreateEmployerIndustry;

namespace IdentityService.API.Mapping.EmployerIndustryMappingProfile;

public class CreateEmployerIndustryRequestToCommandProfile : Profile
{
    public CreateEmployerIndustryRequestToCommandProfile()
    {
        CreateMap<CreateEmployerIndustryRequest, CreateEmployerIndustryCommand>();
    }
}

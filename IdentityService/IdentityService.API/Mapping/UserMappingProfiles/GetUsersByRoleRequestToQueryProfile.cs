using IdentityService.API.Contracts.UserContracts;
using IdentityService.BLL.UseCases.UserUseCases.Queries.GetUsersByRole;

namespace IdentityService.API.Mapping.UserMappingProfiles;

public class GetUsersByRoleRequestToQueryProfile : Profile
{
    public GetUsersByRoleRequestToQueryProfile()
    {
        CreateMap<GetUsersByRoleRequest, GetUsersByRoleQuery>();
    }
}
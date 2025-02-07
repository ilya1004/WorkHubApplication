
using IdentityService.DAL.Constants;

namespace IdentityService.BLL.UseCases.UserUseCases.Queries.GetUsersByRole;

public class GetUsersByRoleQueryHandler : IRequestHandler<GetUsersByRoleQuery, IEnumerable<AppUser>>
{
    private readonly UserManager<AppUser> _userManager;
    public GetUsersByRoleQueryHandler(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<IEnumerable<AppUser>> Handle(GetUsersByRoleQuery request, CancellationToken cancellationToken)
    {
        if (request.UserRole != AppRoles.AdminRole &&
            request.UserRole != AppRoles.EmployerRole &&
            request.UserRole != AppRoles.FreelancerRole) 
        {
            throw new BadRequestException($"User role value must be '{AppRoles.AdminRole}', " +
                $"'{AppRoles.EmployerRole}' or '{AppRoles.FreelancerRole}'");
        }

        var result = await _userManager.GetUsersInRoleAsync(request.UserRole);

        return result;
    }
}

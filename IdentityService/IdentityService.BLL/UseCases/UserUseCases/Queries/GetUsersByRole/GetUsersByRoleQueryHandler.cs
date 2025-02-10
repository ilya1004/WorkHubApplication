using IdentityService.BLL.Models;
using IdentityService.DAL.Constants;

namespace IdentityService.BLL.UseCases.UserUseCases.Queries.GetUsersByRole;

public class GetUsersByRoleQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetUsersByRoleQuery, PaginatedResultModel<AppUser>>
{
    public async Task<PaginatedResultModel<AppUser>> Handle(GetUsersByRoleQuery request, CancellationToken cancellationToken)
    {
        if (request.RoleName != AppRoles.AdminRole &&
            request.RoleName != AppRoles.EmployerRole &&
            request.RoleName != AppRoles.FreelancerRole)
        {
            throw new BadRequestException($"User role value is incorrect. It should be " +
                $"'{AppRoles.AdminRole}', '{AppRoles.EmployerRole}' or '{AppRoles.FreelancerRole}'");
        }

        int offset = (request.PageNo - 1) * request.PageSize;

        var users = await unitOfWork.UsersRepository.PaginatedListAsync(
            u => u.Role.Name == request.RoleName, 
            offset, 
            request.PageSize, 
            cancellationToken, 
            u => u.Role);

        var usersCount = await unitOfWork.UsersRepository.CountAsync(u => u.Role.Name == request.RoleName, cancellationToken);

        return new PaginatedResultModel<AppUser>
        {
            Items = users.ToList(),
            TotalCount = usersCount,
            PageNo = request.PageNo,
            PageSize = request.PageSize
        };
    }
}

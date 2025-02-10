using IdentityService.BLL.Models;
using IdentityService.DAL.Constants;
using Microsoft.AspNetCore.Identity;

namespace IdentityService.BLL.UseCases.UserUseCases.Queries.GetUsersByRole;

public class GetUsersByRoleQueryHandler(
    UserManager<AppUser> userManager,
    IUnitOfWork unitOfWork) : IRequestHandler<GetUsersByRoleQuery, IEnumerable<AppUser>>
{
    public async Task<IEnumerable<AppUser>> Handle(GetUsersByRoleQuery request, CancellationToken cancellationToken)
    {
        //var usersQuery = userManager.Users
        //    .Where(u => u.Roles.Any(r => r.Name == request.RoleName)) // Фильтрация пользователей по роли
        //    .OrderBy(u => u.); // Можно сортировать по Email, ID или дате регистрации

        //var totalCount = await usersQuery.CountAsync(cancellationToken); // Общее число пользователей с ролью
        //var users = await usersQuery
        //    .Skip((request.Page - 1) * request.PageSize)
        //    .Take(request.PageSize)
        //    .ToListAsync(cancellationToken); // Пагинация

        //return new PaginatedResult<AppUser>
        //{
        //    Items = mapper.Map<List<AppUser>>(users),
        //    TotalCount = totalCount,
        //    Page = request.Page,
        //    PageSize = request.PageSize
        //};

        if (request.Role != AppRoles.AdminRole &&
            request.Role != AppRoles.EmployerRole &&
            request.Role != AppRoles.FreelancerRole) 
        {
            throw new BadRequestException($"User role value must be '{AppRoles.AdminRole}', " +
                $"'{AppRoles.EmployerRole}' or '{AppRoles.FreelancerRole}'");
        }

        var users = await unitOfWork.UsersRepository.PaginatedListAsync(u => u.role)

        var result = await userManager.GetUsersInRoleAsync(request.Role);

        return result;
    }


    public async Task<PagedResult<AppUserDto>> Handle(GetUsersByRoleQuery request, CancellationToken cancellationToken)
    {
        var role = await roleManager.FindByNameAsync(request.RoleName);
        if (role == null)
            throw new NotFoundException($"Role '{request.RoleName}' not found");

        var usersQuery = userManager.Users
            .Where(u => u.Roles.Any(r => r.Name == request.RoleName)) // Фильтрация пользователей по роли
            .OrderBy(u => u.Email); // Можно сортировать по Email, ID или дате регистрации

        var totalCount = await usersQuery.CountAsync(cancellationToken); // Общее число пользователей с ролью
        var users = await usersQuery
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken); // Пагинация

        return new PagedResult<AppUserDto>
        {
            Items = _mapper.Map<List<AppUserDto>>(users),
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }
}

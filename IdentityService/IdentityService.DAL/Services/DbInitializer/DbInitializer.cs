
using IdentityService.DAL.Constants;
using IdentityService.DAL.Entities;
using Microsoft.AspNetCore.Identity;

namespace IdentityService.DAL.Services.DbInitializer;

public class DbInitializer : IDbInitializer
{
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;
    public DbInitializer(UserManager<AppUser> userManager, RoleManager<IdentityRole<Guid>> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task InitializeDb()
    {
        if (await _roleManager.FindByNameAsync(AppRoles.AdminRole) == null)
        {
            await _roleManager.CreateAsync(new IdentityRole<Guid>(AppRoles.AdminRole));
            await _roleManager.CreateAsync(new IdentityRole<Guid>(AppRoles.EmployerRole));
            await _roleManager.CreateAsync(new IdentityRole<Guid>(AppRoles.FreelancerRole));
        }
        else
        {
            return;
        }

    }
}

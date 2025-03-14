using IdentityService.DAL.Abstractions.DbInitializer;
using IdentityService.DAL.Abstractions.Repositories;
using IdentityService.DAL.Constants;
using IdentityService.DAL.Entities;
using Microsoft.AspNetCore.Identity;

namespace IdentityService.DAL.Services.DbInitializer;

public class DbInitializer(
    RoleManager<IdentityRole<Guid>> roleManager,
    IUnitOfWork unitOfWork,
    UserManager<AppUser> userManager) : IDbInitializer
{
    public async Task InitializeDb()
    {
        if (await roleManager.FindByNameAsync(AppRoles.AdminRole) is null)
        {
            await roleManager.CreateAsync(new IdentityRole<Guid>(AppRoles.AdminRole));
            await roleManager.CreateAsync(new IdentityRole<Guid>(AppRoles.EmployerRole));
            await roleManager.CreateAsync(new IdentityRole<Guid>(AppRoles.FreelancerRole));
        }
        else
        {
            return;
        }

        var adminRole = await roleManager.FindByNameAsync(AppRoles.AdminRole);

        var admin = new AppUser
        {
            Id = Guid.NewGuid(),
            RegisteredAt = DateTime.UtcNow,
            UserName = "Admin",
            NormalizedUserName = "ADMIN",
            Email = "admin@gmail.com",
            NormalizedEmail = "ADMIN@GMAIL.COM",
            EmailConfirmed = true,
            RoleId = adminRole!.Id
        };

        await userManager.CreateAsync(admin, "Admin_123");

        var freelancerSkills = new List<FreelancerSkill>
        {
            new() { Id = Guid.NewGuid(), Name = "Web Development", NormalizedName = "WEB_DEVELOPMENT" },
            new() { Id = Guid.NewGuid(), Name = "Mobile Development", NormalizedName = "MOBILE_DEVELOPMENT" },
            new() { Id = Guid.NewGuid(), Name = "Graphic Design", NormalizedName = "GRAPHIC_DESIGN" },
            new() { Id = Guid.NewGuid(), Name = "Copywriting", NormalizedName = "COPYWRITING" },
            new() { Id = Guid.NewGuid(), Name = "SEO Optimization", NormalizedName = "SEO_OPTIMIZATION" },
            new() { Id = Guid.NewGuid(), Name = "Project Management", NormalizedName = "PROJECT_MANAGEMENT" },
            new() { Id = Guid.NewGuid(), Name = "Data Analysis", NormalizedName = "DATA_ANALYSIS" }
        };

        foreach (var item in freelancerSkills) await unitOfWork.FreelancerSkillsRepository.AddAsync(item);

        var employerIndustries = new List<EmployerIndustry>
        {
            new() { Id = Guid.NewGuid(), Name = "IT & Software", NormalizedName = "IT_SOFTWARE" },
            new() { Id = Guid.NewGuid(), Name = "Marketing & Advertising", NormalizedName = "MARKETING_ADVERTISING" },
            new() { Id = Guid.NewGuid(), Name = "Finance & Accounting", NormalizedName = "FINANCE_ACCOUNTING" },
            new() { Id = Guid.NewGuid(), Name = "Healthcare & Medicine", NormalizedName = "HEALTHCARE_MEDICINE" },
            new() { Id = Guid.NewGuid(), Name = "Education & Training", NormalizedName = "EDUCATION_TRAINING" },
            new() { Id = Guid.NewGuid(), Name = "Construction & Engineering", NormalizedName = "CONSTRUCTION_ENGINEERING" },
            new() { Id = Guid.NewGuid(), Name = "E-commerce & Retail", NormalizedName = "ECOMMERCE_RETAIL" }
        };

        foreach (var item in employerIndustries) await unitOfWork.EmployerIndustriesRepository.AddAsync(item);

        await unitOfWork.SaveAllAsync();
    }
}
// This is only a test data, which initialization will be moved to HasData() method lately
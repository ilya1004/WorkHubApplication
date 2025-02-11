using IdentityService.DAL.Abstractions.Repositories;
using IdentityService.DAL.Constants;
using IdentityService.DAL.Entities;
using Microsoft.AspNetCore.Identity;

namespace IdentityService.DAL.Services.DbInitializer;

public class DbInitializer(
    RoleManager<IdentityRole<Guid>> roleManager,
    IUnitOfWork unitOfWork) : IDbInitializer
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

        var freelancerSkills = new List<FreelancerSkill>
        {
            new FreelancerSkill { Id = Guid.NewGuid(), Name = "Web Development", NormalizedName = "WEB_DEVELOPMENT" },
            new FreelancerSkill { Id = Guid.NewGuid(), Name = "Mobile Development", NormalizedName = "MOBILE_DEVELOPMENT" },
            new FreelancerSkill { Id = Guid.NewGuid(), Name = "Graphic Design", NormalizedName = "GRAPHIC_DESIGN" },
            new FreelancerSkill { Id = Guid.NewGuid(), Name = "Copywriting", NormalizedName = "COPYWRITING" },
            new FreelancerSkill { Id = Guid.NewGuid(), Name = "SEO Optimization", NormalizedName = "SEO_OPTIMIZATION" },
            new FreelancerSkill { Id = Guid.NewGuid(), Name = "Project Management", NormalizedName = "PROJECT_MANAGEMENT" },
            new FreelancerSkill { Id = Guid.NewGuid(), Name = "Data Analysis", NormalizedName = "DATA_ANALYSIS" }
        };

        foreach (var item in freelancerSkills)
        {
            await unitOfWork.FreelancerSkillsRepository.AddAsync(item);
        }

        var employerIndustries = new List<EmployerIndustry>
        {
            new EmployerIndustry { Id = Guid.NewGuid(), Name = "IT & Software", NormalizedName = "IT_SOFTWARE" },
            new EmployerIndustry { Id = Guid.NewGuid(), Name = "Marketing & Advertising", NormalizedName = "MARKETING_ADVERTISING" },
            new EmployerIndustry { Id = Guid.NewGuid(), Name = "Finance & Accounting", NormalizedName = "FINANCE_ACCOUNTING" },
            new EmployerIndustry { Id = Guid.NewGuid(), Name = "Healthcare & Medicine", NormalizedName = "HEALTHCARE_MEDICINE" },
            new EmployerIndustry { Id = Guid.NewGuid(), Name = "Education & Training", NormalizedName = "EDUCATION_TRAINING" },
            new EmployerIndustry { Id = Guid.NewGuid(), Name = "Construction & Engineering", NormalizedName = "CONSTRUCTION_ENGINEERING" },
            new EmployerIndustry { Id = Guid.NewGuid(), Name = "E-commerce & Retail", NormalizedName = "ECOMMERCE_RETAIL" }
        };

        foreach (var item in employerIndustries)
        {
            await unitOfWork.EmployerIndustriesRepository.AddAsync(item);
        }

        await unitOfWork.SaveAllAsync();
    }
}
// This is only a test data, which initialization will be moved to HasData method lately
using IdentityService.DAL.Abstractions.DbStartupService;
using IdentityService.DAL.Abstractions.Repositories;
using IdentityService.DAL.Constants;
using IdentityService.DAL.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace IdentityService.DAL.Services.DbStartupService;

public class DbStartupService(
    IServiceProvider serviceProvider,
    RoleManager<IdentityRole<Guid>> roleManager,
    IUnitOfWork unitOfWork,
    UserManager<AppUser> userManager,
    ILogger<DbStartupService> logger) : IDbStartupService
{
    public async Task MakeMigrationsAsync()
    {
        logger.LogInformation("Starting database migrations...");
        
        using var scope = serviceProvider.CreateScope();
        await using var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        try
        {
            await dbContext.Database.MigrateAsync();
            
            logger.LogInformation("Database migrations applied successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error applying database migrations");

            throw new Exception("Error applying database migrations", ex);
        }
    }

    public async Task InitializeDb()
    {
        logger.LogInformation("Starting database initialization...");

        var adminRole = await roleManager.FindByNameAsync(AppRoles.AdminRole);

        if (adminRole is null)
        {
            logger.LogInformation("Creating application roles...");
            
            await roleManager.CreateAsync(new IdentityRole<Guid>(AppRoles.AdminRole));
            await roleManager.CreateAsync(new IdentityRole<Guid>(AppRoles.EmployerRole));
            await roleManager.CreateAsync(new IdentityRole<Guid>(AppRoles.FreelancerRole));
            
            await unitOfWork.SaveAllAsync();
            
            logger.LogInformation("Application roles created successfully");
        }
        else
        {
            logger.LogInformation("Roles already exist, skipping creation");
            
            return;
        }
        
        logger.LogInformation("Creating default admin user...");
        
        adminRole = await roleManager.FindByNameAsync(AppRoles.AdminRole);

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

        var createResultAdmin = await userManager.CreateAsync(admin, "Admin_123");
        
        if (!createResultAdmin.Succeeded)
        {
            logger.LogError("Failed to create admin user.");
            
            throw new Exception("Failed to create admin user.");
        }
        
        logger.LogInformation("Seeding freelancer skills...");
        
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

        foreach (var item in freelancerSkills) 
        {
            await unitOfWork.FreelancerSkillsRepository.AddAsync(item);
        }
        
        logger.LogInformation("Seeding employer industries...");
        
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
        
        foreach (var item in employerIndustries) 
        {
            await unitOfWork.EmployerIndustriesRepository.AddAsync(item);
        }
        
        await unitOfWork.SaveAllAsync();
        
        var freelancerRole = await roleManager.FindByNameAsync(AppRoles.FreelancerRole);
        
        logger.LogInformation("Creating freelancer user...");
        
        var freelancer = new AppUser
        {
            Id = Guid.NewGuid(),
            RegisteredAt = DateTime.UtcNow,
            UserName = "Moonlight",
            NormalizedUserName = "MOONLIGHT",
            Email = "ilya@gmail.com",
            NormalizedEmail = "ILYA@GMAIL.COM",
            EmailConfirmed = true,
            RoleId = freelancerRole!.Id
        };
        
        var createResultFreelancer = await userManager.CreateAsync(freelancer, "Ilya_123");
        
        if (!createResultFreelancer.Succeeded)
        {
            logger.LogError("Failed to create freelancer user.");
            
            throw new Exception($"Failed to create freelancer user.");
        }
        
        var freelancerProfile = new FreelancerProfile
        {
            FirstName = "Ilya",
            LastName = "Rabets",
            About = "I'm Ilya Rabets!",
            Skills = freelancerSkills.Take(4).ToList(),
            UserId = freelancer.Id
        };
        
        await unitOfWork.FreelancerProfilesRepository.AddAsync(freelancerProfile);
        
        logger.LogInformation("Freelancer user created successfully with ID: {FreelancerId}", freelancer.Id);
        
        var employerRole = await roleManager.FindByNameAsync(AppRoles.EmployerRole);
        
        logger.LogInformation("Creating employer user...");
        
        var employer = new AppUser
        {
            Id = Guid.NewGuid(),
            RegisteredAt = DateTime.UtcNow,
            UserName = "Pavlusha",
            NormalizedUserName = "PAVLUSHA",
            Email = "pavlusha@gmail.com",
            NormalizedEmail = "PAVLUSHA@GMAIL.COM",
            EmailConfirmed = true,
            RoleId = employerRole!.Id
        };
        
        var createResultEmployer = await userManager.CreateAsync(employer, "Pavlusha_123");
        
        if (!createResultEmployer.Succeeded)
        {
            logger.LogError("Failed to create employer user.");
            
            throw new Exception("Failed to create employer user.");
        }
        
        var employerProfile = new EmployerProfile
        {
            CompanyName = "Sunrise Company",
            About = "We are Sunrise Company!",
            Industry = employerIndustries.First(),
            UserId = employer.Id
        };
        
        await unitOfWork.EmployerProfilesRepository.AddAsync(employerProfile);
        
        logger.LogInformation("Employer user created successfully with ID: {AdminId}", admin.Id);
        
        await unitOfWork.SaveAllAsync();
        
        logger.LogInformation("Database initialization completed successfully");
    }
}
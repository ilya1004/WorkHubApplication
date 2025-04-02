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
            
            logger.LogInformation("Application roles created successfully");
        }
        else
        {
            logger.LogInformation("Roles already exist, skipping creation");
            
            return;
        }

        adminRole = await roleManager.FindByNameAsync(AppRoles.AdminRole);
        
        logger.LogInformation("Creating default admin user...");

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

        var createResult = await userManager.CreateAsync(admin, "Admin_123");
        if (!createResult.Succeeded)
        {
            var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
            
            logger.LogError("Failed to create admin user. Errors: {Errors}", errors);
            
            throw new Exception($"Failed to create admin user: {errors}");
        }

        logger.LogInformation("Default admin user created successfully with ID: {AdminId}", admin.Id);

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
        logger.LogInformation("Database initialization completed successfully");
    }
}
using Microsoft.Extensions.DependencyInjection;
using ProjectsService.Domain.Abstractions.StartupServices;
using ProjectsService.Domain.Enums;
using ProjectsService.Infrastructure.Data;

namespace ProjectsService.Infrastructure.Services.DbStartupService;

public class DbStartupService(
    IServiceProvider serviceProvider,
    IUnitOfWork unitOfWork,
    ILogger<DbStartupService> logger) : IDbStartupService
{
    private const string EmployerId = "e13341b4-6532-41f6-9595-202525c7ff34";
    private const string FreelancerId = "52d78d21-8f4d-469d-a911-b094d6f9994b";
    
    public async Task MakeMigrationsAsync()
    {
        logger.LogInformation("Starting database migrations...");
        
        using var scope = serviceProvider.CreateScope();
        await using var dbContext = scope.ServiceProvider.GetRequiredService<CommandsDbContext>();

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
        
        var existingCategories = await unitOfWork.CategoryQueriesRepository.ListAsync(c => true);
        if (existingCategories.Any())
        {
            logger.LogInformation("Database already initialized, skipping seeding");
            return;
        }

        logger.LogInformation("Seeding categories...");
        
        var categories = new List<Category>
        {
            new() { Id = Guid.NewGuid(), Name = "Web Development", NormalizedName = "WEB_DEVELOPMENT" },
            new() { Id = Guid.NewGuid(), Name = "Mobile Development", NormalizedName = "MOBILE_DEVELOPMENT" },
            new() { Id = Guid.NewGuid(), Name = "Graphic Design", NormalizedName = "GRAPHIC_DESIGN" },
            new() { Id = Guid.NewGuid(), Name = "Marketing", NormalizedName = "MARKETING" },
            new() { Id = Guid.NewGuid(), Name = "Data Analysis", NormalizedName = "DATA_ANALYSIS" }
        };

        foreach (var category in categories)
        {
            await unitOfWork.CategoryCommandsRepository.AddAsync(category);
        }
        
        logger.LogInformation("Seeding projects...");

        var employerId = Guid.Parse(EmployerId);
        var freelancerId = Guid.Parse(FreelancerId);

        var projects = new List<Project>
        {
            new Project
            {
                Id = Guid.NewGuid(),
                Title = "E-commerce Website",
                Description = "Develop a responsive e-commerce website",
                Budget = 5000m,
                CategoryId = categories[0].Id,
                EmployerId = employerId,
                FreelancerId = null,
                Lifecycle = new Lifecycle
                {
                    Id = Guid.NewGuid(),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    ApplicationsStartDate = DateTime.UtcNow,
                    ApplicationsDeadline = DateTime.UtcNow.AddDays(7),
                    WorkStartDate = DateTime.UtcNow.AddDays(10),
                    WorkDeadline = DateTime.UtcNow.AddDays(40),
                    AcceptanceRequested = false,
                    AcceptanceConfirmed = false,
                    Status = ProjectStatus.InProgress
                },
                FreelancerApplications = new List<FreelancerApplication>
                {
                    new FreelancerApplication
                    {
                        Id = Guid.NewGuid(),
                        CreatedAt = DateTime.UtcNow,
                        Status = ApplicationStatus.Pending,
                        FreelancerId = freelancerId
                    }
                }
            },
            new Project
            {
                Id = Guid.NewGuid(),
                Title = "Mobile Banking App",
                Description = "Create a secure mobile banking application",
                Budget = 8000m,
                CategoryId = categories[1].Id,
                EmployerId = employerId,
                FreelancerId = freelancerId,
                Lifecycle = new Lifecycle
                {
                    Id = Guid.NewGuid(),
                    CreatedAt = DateTime.UtcNow.AddDays(-10),
                    UpdatedAt = DateTime.UtcNow.AddDays(-2),
                    ApplicationsStartDate = DateTime.UtcNow.AddDays(-9),
                    ApplicationsDeadline = DateTime.UtcNow.AddDays(-2),
                    WorkStartDate = DateTime.UtcNow.AddDays(-2),
                    WorkDeadline = DateTime.UtcNow.AddDays(58),
                    AcceptanceRequested = false,
                    AcceptanceConfirmed = false,
                    Status = ProjectStatus.InProgress
                },
                FreelancerApplications = new List<FreelancerApplication>(),
            }
        };

        foreach (var project in projects)
        {
            await unitOfWork.ProjectCommandsRepository.AddAsync(project);
            
            project.Lifecycle.ProjectId = project.Id;
            await unitOfWork.LifecycleCommandsRepository.AddAsync(project.Lifecycle);
            
            if (project.FreelancerApplications.Count != 0)
            {
                foreach (var application in project.FreelancerApplications)
                {
                    application.ProjectId = project.Id;
                    await unitOfWork.FreelancerApplicationCommandsRepository.AddAsync(application);
                }
            }
        }

        await unitOfWork.SaveAllAsync();

        logger.LogInformation("Database initialization completed successfully");
    }
}
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace IdentityService.DAL.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>(options)
{
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }

    public DbSet<AppUser> AppUsers { get; set; }
    public DbSet<FreelancerProfile> FreelancerProfiles { get; set; }
    public DbSet<FreelancerSkill> FreelancerSkills { get; set; }
    public DbSet<EmployerProfile> EmployerProfiles { get; set; }
    public DbSet<EmployerIndustry> EmployerIndustries { get; set; }
}
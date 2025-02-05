using IdentityService.DAL.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.DAL.Data;

public class ApplicationDbContext : IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

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

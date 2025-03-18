namespace ProjectsService.Infrastructure.Data;

public class QueriesDbContext(DbContextOptions<QueriesDbContext> options) : DbContext(options)
{
    public DbSet<Category> Categories { get; set; }
    public DbSet<FreelancerApplication> FreelancerApplications { get; set; }
    public DbSet<Lifecycle> Lifecycles { get; set; }
    public DbSet<Project> Projects { get; set; }
}
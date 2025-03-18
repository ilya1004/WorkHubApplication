namespace ProjectsService.Infrastructure.Data;

public class CommandsDbContext(DbContextOptions<CommandsDbContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(typeof(CommandsDbContext).Assembly);
    }
    
    public DbSet<Category> Categories { get; set; }
    public DbSet<FreelancerApplication> FreelancerApplications { get; set; }
    public DbSet<Lifecycle> Lifecycles { get; set; }
    public DbSet<Project> Projects { get; set; }
}
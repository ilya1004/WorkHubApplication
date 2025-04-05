namespace ProjectsService.Domain.Abstractions.StartupServices;

public interface IDbStartupService
{
    Task MakeMigrationsAsync();
    Task InitializeDb();
}
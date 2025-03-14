namespace ProjectsService.Domain.Abstractions.StartupServices;

public interface IBackgroundJobsInitializer
{
    void StartBackgroundJobs();
}
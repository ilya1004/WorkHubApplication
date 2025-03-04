using Hangfire;
using ProjectsService.Application.Abstractions.BackgroundJobs;
using ProjectsService.Application.BackgroundJobs.UpdateProjectStatuses;
using ProjectsService.Domain.Abstractions.StartupServices;

namespace ProjectsService.Infrastructure.Services.HangfireJobsInitializer;

public class HangfireJobsInitializer(IBackgroundJobScheduler scheduler) : IBackgroundJobsInitializer
{
    public void StartBackgroundJobs()
    {
        scheduler.ScheduleRecurring("update_project_statuses", new UpdateProjectStatusesCommand(), Cron.Hourly());
    }
}

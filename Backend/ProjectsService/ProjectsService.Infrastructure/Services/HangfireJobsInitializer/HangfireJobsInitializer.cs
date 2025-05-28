using Hangfire;
using MediatR;
using ProjectsService.Application.BackgroundJobs.UpdateProjectStatuses;
using ProjectsService.Domain.Abstractions.StartupServices;

namespace ProjectsService.Infrastructure.Services.HangfireJobsInitializer;

public class HangfireJobsInitializer(
    IRecurringJobManager recurringJobManager,
    IMediator mediator,
    ILogger<HangfireJobsInitializer> logger) : IBackgroundJobsInitializer
{
    public void StartBackgroundJobs()
    {
        logger.LogInformation("Starting background jobs initialization");
        
        recurringJobManager.AddOrUpdate(
            "update_project_statuses",
            () => mediator.Send(new UpdateProjectStatusesCommand(), CancellationToken.None),
            Cron.Hourly()
        );
        
        logger.LogInformation("Recurring job 'update_project_statuses' scheduled to run every minute");
    }
}
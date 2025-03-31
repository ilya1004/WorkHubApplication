using Hangfire;
using MediatR;
using ProjectsService.Application.BackgroundJobs.UpdateProjectStatuses;
using ProjectsService.Domain.Abstractions.StartupServices;

namespace ProjectsService.Infrastructure.Services.HangfireJobsInitializer;

public class HangfireJobsInitializer(
    IRecurringJobManager recurringJobManager,
    IMediator mediator) : IBackgroundJobsInitializer
{
    public void StartBackgroundJobs()
    {
        recurringJobManager.AddOrUpdate(
            "update_project_statuses",
            () => mediator.Send(new UpdateProjectStatusesCommand(), CancellationToken.None),
            Cron.Minutely()
        );
    }
}

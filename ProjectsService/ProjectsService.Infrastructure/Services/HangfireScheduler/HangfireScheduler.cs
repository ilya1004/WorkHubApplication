using Hangfire;
using MediatR;
using ProjectsService.Application.Abstractions.BackgroundJobs;

namespace ProjectsService.Infrastructure.Services.HangfireScheduler;

public class HangfireScheduler(IMediator mediator) : IBackgroundJobScheduler
{
    public void ScheduleRecurring<TRequest>(string jobId, TRequest request, string cronExpression) where TRequest : IRequest
    {
        RecurringJob.AddOrUpdate(jobId, () => mediator.Send(request, CancellationToken.None), cronExpression);
    }
}

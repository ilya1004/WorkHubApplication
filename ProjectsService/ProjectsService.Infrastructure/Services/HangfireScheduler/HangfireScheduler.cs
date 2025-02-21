using Hangfire;
using MediatR;
using ProjectsService.Application.Abstractions.BackgroundJobs;

namespace ProjectsService.Infrastructure.Services.HangfireScheduler;

public class HangfireScheduler(IMediator mediator) : IBackgroundJobScheduler
{
    public void Enqueue<TRequest>(TRequest request) where TRequest : IRequest
    {
        BackgroundJob.Enqueue(() => mediator.Send(request, CancellationToken.None));
    }
    
    public void Schedule<TRequest>(TRequest request, TimeSpan delay) where TRequest : IRequest
    {
        BackgroundJob.Schedule(() => mediator.Send(request, CancellationToken.None), delay);
    }
    
    public void ScheduleRecurring<TRequest>(string jobId, TRequest request, string cronExpression) where TRequest : IRequest
    {
        RecurringJob.AddOrUpdate(jobId, () => mediator.Send(request, CancellationToken.None), cronExpression);
    }
    
    public void ContinueWith<TFirstRequest, TSecondRequest>(TFirstRequest firstRequest, TSecondRequest secondRequest)
        where TFirstRequest : IRequest
        where TSecondRequest : IRequest
    {
        var parentJobId = BackgroundJob.Enqueue(() => mediator.Send(firstRequest, CancellationToken.None));

        BackgroundJob.ContinueJobWith(parentJobId, () => mediator.Send(secondRequest, CancellationToken.None));
    }
}

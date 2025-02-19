namespace ProjectsService.Application.Abstractions.BackgroundJobs;

public interface IBackgroundJobScheduler
{
    void Enqueue<TRequest>(TRequest request) where TRequest : IRequest;
    void Schedule<TRequest>(TRequest request, TimeSpan delay) where TRequest : IRequest;
    void ScheduleRecurring<TRequest>(string jobId, TRequest request, string cronExpression) where TRequest : IRequest;
    void ContinueWith<TFirstRequest, TSecondRequest>(TFirstRequest firstRequest, TSecondRequest secondRequest)
        where TFirstRequest : IRequest
        where TSecondRequest : IRequest;
}
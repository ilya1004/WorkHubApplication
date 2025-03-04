namespace ProjectsService.Application.Abstractions.BackgroundJobs;

public interface IBackgroundJobScheduler
{
    void ScheduleRecurring<TRequest>(string jobId, TRequest request, string cronExpression) where TRequest : IRequest;
}
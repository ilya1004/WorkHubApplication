using ProjectsService.Domain.Entities;

namespace ProjectsService.Domain.Abstractions.Data;

public interface IUnitOfWork
{
    ICommandsRepository<Category> CategoryCommandsRepository { get; }
    IQueriesRepository<Category> CategoryQueriesRepository { get; }
    ICommandsRepository<FreelancerApplication> FreelancerApplicationCommandsRepository { get; }
    IQueriesRepository<FreelancerApplication> FreelancerApplicationQueriesRepository { get; }
    ICommandsRepository<Lifecycle> LifecycleCommandsRepository { get; }
    IQueriesRepository<Lifecycle> LifecycleQueriesRepository { get; }
    ICommandsRepository<Project> ProjectCommandsRepository { get; }
    IQueriesRepository<Project> ProjectQueriesRepository { get; }
    public Task SaveAllAsync(CancellationToken cancellationToken = default);
}
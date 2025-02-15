using ProjectsService.Domain.Abstractions.Data;
using ProjectsService.Infrastructure.Data;

namespace ProjectsService.Infrastructure.Repositories;

public class AppUnitOfWork(
    CommandsDbContext commandsDbContext, 
    QueriesDbContext queriesDbContext) : IUnitOfWork
{
    private readonly Lazy<ICommandsRepository<Category>> _categoryCommandsRepository = 
        new(() => new CommandsRepository<Category>(commandsDbContext));
    private readonly Lazy<IQueriesRepository<Category>> _categoryQueriesRepository =
        new(() => new QueriesRepository<Category>(queriesDbContext));
    private readonly Lazy<ICommandsRepository<FreelancerApplication>> _freelancerApplicationCommandsRepository =
        new(() => new CommandsRepository<FreelancerApplication>(commandsDbContext));
    private readonly Lazy<IQueriesRepository<FreelancerApplication>> _freelancerApplicationQueriesRepository =
        new(() => new QueriesRepository<FreelancerApplication>(queriesDbContext));
    private readonly Lazy<ICommandsRepository<Lifecycle>> _lifecycleCommandsRepository =
        new(() => new CommandsRepository<Lifecycle>(commandsDbContext));
    private readonly Lazy<IQueriesRepository<Lifecycle>> _lifecycleQueriesRepository =
        new(() => new QueriesRepository<Lifecycle>(queriesDbContext));
    private readonly Lazy<ICommandsRepository<Project>> _projectCommandsRepository =
        new(() => new CommandsRepository<Project>(commandsDbContext));
    private readonly Lazy<IQueriesRepository<Project>> _projectQueriesRepository =
        new(() => new QueriesRepository<Project>(queriesDbContext));
    
    public ICommandsRepository<Category> CategoryCommandsRepository => _categoryCommandsRepository.Value;
    public IQueriesRepository<Category> CategoryQueriesRepository => _categoryQueriesRepository.Value;
    public ICommandsRepository<FreelancerApplication> FreelancerApplicationCommandsRepository 
        => _freelancerApplicationCommandsRepository.Value;
    public IQueriesRepository<FreelancerApplication> FreelancerApplicationQueriesRepository 
        => _freelancerApplicationQueriesRepository.Value;
    public ICommandsRepository<Lifecycle> LifecycleCommandsRepository => _lifecycleCommandsRepository.Value;
    public IQueriesRepository<Lifecycle> LifecycleQueriesRepository => _lifecycleQueriesRepository.Value;
    public ICommandsRepository<Project> ProjectCommandsRepository => _projectCommandsRepository.Value;
    public IQueriesRepository<Project> ProjectQueriesRepository => _projectQueriesRepository.Value;

    public async Task SaveAllAsync(CancellationToken cancellationToken = default)
    {
        await commandsDbContext.SaveChangesAsync(cancellationToken);
    }
}

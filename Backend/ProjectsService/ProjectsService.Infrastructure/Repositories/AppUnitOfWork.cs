using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using ProjectsService.Infrastructure.Data;

namespace ProjectsService.Infrastructure.Repositories;

public class AppUnitOfWork(
    CommandsDbContext commandsDbContext, 
    QueriesDbContext queriesDbContext,
    IDistributedCache distributedCache,
    IOptions<CacheOptions> options) : IUnitOfWork
{
    private readonly Lazy<ICommandsRepository<Category>> _categoryCommandsRepository = 
        new(() => new CachedCommandsRepository<Category>(new CommandsRepository<Category>(commandsDbContext), distributedCache));
    
    private readonly Lazy<IQueriesRepository<Category>> _categoryQueriesRepository =
        new(() => new CachedQueriesRepository<Category>(new QueriesRepository<Category>(queriesDbContext), distributedCache, options));
    
    private readonly Lazy<ICommandsRepository<FreelancerApplication>> _freelancerApplicationCommandsRepository =
        new(() => new CachedCommandsRepository<FreelancerApplication>(
            new CommandsRepository<FreelancerApplication>(commandsDbContext), distributedCache));
    
    private readonly Lazy<IQueriesRepository<FreelancerApplication>> _freelancerApplicationQueriesRepository =
        new(() => new CachedQueriesRepository<FreelancerApplication>(
            new QueriesRepository<FreelancerApplication>(queriesDbContext), distributedCache, options));
    
    private readonly Lazy<ICommandsRepository<Lifecycle>> _lifecycleCommandsRepository =
        new(() => new CachedCommandsRepository<Lifecycle>(new CommandsRepository<Lifecycle>(commandsDbContext), distributedCache));
    
    private readonly Lazy<IQueriesRepository<Lifecycle>> _lifecycleQueriesRepository =
        new(() => new CachedQueriesRepository<Lifecycle>(new QueriesRepository<Lifecycle>(queriesDbContext), distributedCache, options));
    
    private readonly Lazy<ICommandsRepository<Project>> _projectCommandsRepository =
        new(() => new CachedCommandsRepository<Project>(new CommandsRepository<Project>(commandsDbContext), distributedCache));
    
    private readonly Lazy<IQueriesRepository<Project>> _projectQueriesRepository =
        new(() => new CachedQueriesRepository<Project>(new QueriesRepository<Project>(queriesDbContext), distributedCache, options));
    
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

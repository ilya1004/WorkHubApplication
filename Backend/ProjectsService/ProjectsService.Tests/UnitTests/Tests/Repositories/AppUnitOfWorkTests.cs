using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using ProjectsService.Infrastructure.Data;
using ProjectsService.Infrastructure.Repositories;
using ProjectsService.Infrastructure.Settings;

namespace ProjectsService.Tests.UnitTests.Tests.Repositories;

public class AppUnitOfWorkTests
{
    private readonly CommandsDbContext _commandsContext;
    private readonly AppUnitOfWork _unitOfWork;

    public AppUnitOfWorkTests()
    {
        var commandsOptions = new DbContextOptionsBuilder<CommandsDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        var queriesOptions = new DbContextOptionsBuilder<QueriesDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _commandsContext = new CommandsDbContext(commandsOptions);
        var queriesContext = new QueriesDbContext(queriesOptions);
        var cacheMock = new Mock<IDistributedCache>();
        var cacheOptions = Options.Create(new CacheOptions { RecordExpirationTimeInMinutes = 30 });

        _unitOfWork = new AppUnitOfWork(_commandsContext, queriesContext, cacheMock.Object, cacheOptions);
    }

    [Fact]
    public async Task SaveAllAsync_SavesChangesInCommandsContext()
    {
        // Arrange
        var project = new Project
        {
            Id = Guid.NewGuid(), 
            Title = "Test Project",
            Description = "Test Description"
        };
        await _commandsContext.Set<Project>().AddAsync(project);

        // Act
        await _unitOfWork.SaveAllAsync();

        // Assert
        var savedProject = await _commandsContext.Set<Project>().FindAsync(project.Id);
        savedProject.Should().BeEquivalentTo(project);
    }

    [Fact]
    public void RepositoryProperties_ReturnInitializedRepositories()
    {
        // Assert
        _unitOfWork.CategoryCommandsRepository.Should().NotBeNull();
        _unitOfWork.CategoryQueriesRepository.Should().NotBeNull();
        _unitOfWork.FreelancerApplicationCommandsRepository.Should().NotBeNull();
        _unitOfWork.FreelancerApplicationQueriesRepository.Should().NotBeNull();
        _unitOfWork.LifecycleCommandsRepository.Should().NotBeNull();
        _unitOfWork.LifecycleQueriesRepository.Should().NotBeNull();
        _unitOfWork.ProjectCommandsRepository.Should().NotBeNull();
        _unitOfWork.ProjectQueriesRepository.Should().NotBeNull();
    }

    [Fact]
    public void Repositories_AreCachedRepositories()
    {
        // Assert
        _unitOfWork.CategoryCommandsRepository.Should().BeOfType<CachedCommandsRepository<Category>>();
        _unitOfWork.CategoryQueriesRepository.Should().BeOfType<CachedQueriesRepository<Category>>();
        _unitOfWork.FreelancerApplicationCommandsRepository.Should().BeOfType<CachedCommandsRepository<FreelancerApplication>>();
        _unitOfWork.FreelancerApplicationQueriesRepository.Should().BeOfType<CachedQueriesRepository<FreelancerApplication>>();
        _unitOfWork.LifecycleCommandsRepository.Should().BeOfType<CachedCommandsRepository<Lifecycle>>();
        _unitOfWork.LifecycleQueriesRepository.Should().BeOfType<CachedQueriesRepository<Lifecycle>>();
        _unitOfWork.ProjectCommandsRepository.Should().BeOfType<CachedCommandsRepository<Project>>();
        _unitOfWork.ProjectQueriesRepository.Should().BeOfType<CachedQueriesRepository<Project>>();
    }
}
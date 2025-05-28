using System.Linq.Expressions;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using ProjectsService.Domain.Abstractions.Data;
using ProjectsService.Infrastructure.Repositories;
using ProjectsService.Infrastructure.Settings;

namespace ProjectsService.Tests.UnitTests.Tests.Repositories;

public class CachedQueriesRepositoryTests
{
    private readonly Mock<IQueriesRepository<Project>> _innerRepositoryMock;
    private readonly Mock<IDistributedCache> _cacheMock;
    private readonly CachedQueriesRepository<Project> _repository;

    public CachedQueriesRepositoryTests()
    {
        _innerRepositoryMock = new Mock<IQueriesRepository<Project>>();
        _cacheMock = new Mock<IDistributedCache>();
        var cacheOptions = Options.Create(new CacheOptions { RecordExpirationTimeInMinutes = 30 });
        _repository = new CachedQueriesRepository<Project>(_innerRepositoryMock.Object, _cacheMock.Object, cacheOptions);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsCachedEntity_WhenExistsInCache()
    {
        // Arrange
        var project = new Project { Id = Guid.NewGuid(), Title = "Test Project" };
        var cacheKey = $"Project:{project.Id}";
        var serializedProject = JsonSerializer.Serialize(project);
        var serializedProjectBytes = Encoding.UTF8.GetBytes(serializedProject);

        _cacheMock.Setup(c => c.GetAsync(cacheKey, It.IsAny<CancellationToken>()))
            .ReturnsAsync(serializedProjectBytes);

        // Act
        var result = await _repository.GetByIdAsync(project.Id);

        // Assert
        result.Should().BeEquivalentTo(project);
        _innerRepositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<Project, object>>[]>()), Times.Never());
    }

    [Fact]
    public async Task GetByIdAsync_FetchesFromRepositoryAndCaches_WhenNotInCache()
    {
        // Arrange
        var project = new Project { Id = Guid.NewGuid(), Title = "Test Project" };
        var cacheKey = $"Project:{project.Id}";

        _cacheMock.Setup(c => c.GetAsync(cacheKey, It.IsAny<CancellationToken>()))
            .ReturnsAsync((byte[]?)null);
        _innerRepositoryMock.Setup(r => r.GetByIdAsync(project.Id, It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<Project, object>>[]>()))
            .ReturnsAsync(project);

        // Act
        var result = await _repository.GetByIdAsync(project.Id);

        // Assert
        result.Should().BeEquivalentTo(project);
        _innerRepositoryMock.Verify(r => r.GetByIdAsync(project.Id, It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<Project, object>>[]>()), Times.Once());
        _cacheMock.Verify(c => c.SetAsync(cacheKey, It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task ListAllAsync_ReturnsCachedEntities_WhenExistsInCache()
    {
        // Arrange
        var projects = new List<Project>
        {
            new() { Id = Guid.NewGuid(), Title = "Project 1" },
            new() { Id = Guid.NewGuid(), Title = "Project 2" }
        };
        var cacheKey = "Project:ListAll";
        var serializedProjects = JsonSerializer.Serialize(projects);
        var serializedProjectsBytes = Encoding.UTF8.GetBytes(serializedProjects);

        _cacheMock.Setup(c => c.GetAsync(cacheKey, It.IsAny<CancellationToken>()))
            .ReturnsAsync(serializedProjectsBytes);

        // Act
        var result = await _repository.ListAllAsync();

        // Assert
        result.Should().BeEquivalentTo(projects);
        _innerRepositoryMock.Verify(r => r.ListAllAsync(It.IsAny<CancellationToken>()), Times.Never());
    }

    [Fact]
    public async Task ListAllAsync_FetchesFromRepositoryAndCaches_WhenNotInCache()
    {
        // Arrange
        var projects = new List<Project>
        {
            new() { Id = Guid.NewGuid(), Title = "Project 1" },
            new() { Id = Guid.NewGuid(), Title = "Project 2" }
        };
        var cacheKey = "Project:ListAll";

        _cacheMock.Setup(c => c.GetAsync(cacheKey, It.IsAny<CancellationToken>()))
            .ReturnsAsync((byte[]?)null);
        _innerRepositoryMock.Setup(r => r.ListAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(projects);

        // Act
        var result = await _repository.ListAllAsync();

        // Assert
        result.Should().BeEquivalentTo(projects);
        _innerRepositoryMock.Verify(r => r.ListAllAsync(It.IsAny<CancellationToken>()), Times.Once());
        _cacheMock.Verify(c => c.SetAsync(cacheKey, It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task CountAllAsync_ReturnsCachedCount_WhenExistsInCache()
    {
        // Arrange
        var cacheKey = "Project:CountAll";
        var count = 42;
        var countString = count.ToString();
        var countBytes = Encoding.UTF8.GetBytes(countString);

        _cacheMock.Setup(c => c.GetAsync(cacheKey, It.IsAny<CancellationToken>()))
            .ReturnsAsync(countBytes);

        // Act
        var result = await _repository.CountAllAsync();

        // Assert
        result.Should().Be(count);
        _innerRepositoryMock.Verify(r => r.CountAllAsync(It.IsAny<CancellationToken>()), Times.Never());
    }

    [Fact]
    public async Task CountAllAsync_FetchesFromRepositoryAndCaches_WhenNotInCache()
    {
        // Arrange
        var cacheKey = "Project:CountAll";
        var count = 42;

        _cacheMock.Setup(c => c.GetAsync(cacheKey, It.IsAny<CancellationToken>()))
            .ReturnsAsync((byte[]?)null);
        _innerRepositoryMock.Setup(r => r.CountAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(count);

        // Act
        var result = await _repository.CountAllAsync();

        // Assert
        result.Should().Be(count);
        _innerRepositoryMock.Verify(r => r.CountAllAsync(It.IsAny<CancellationToken>()), Times.Once());
    }
}
using Microsoft.Extensions.Caching.Distributed;
using ProjectsService.Domain.Abstractions.Data;
using ProjectsService.Infrastructure.Repositories;

namespace ProjectsService.Tests.UnitTests.Tests.Repositories;

public class CachedCommandsRepositoryTests
{
    private readonly Mock<ICommandsRepository<Project>> _innerRepositoryMock;
    private readonly Mock<IDistributedCache> _cacheMock;
    private readonly CachedCommandsRepository<Project> _repository;

    public CachedCommandsRepositoryTests()
    {
        _innerRepositoryMock = new Mock<ICommandsRepository<Project>>();
        _cacheMock = new Mock<IDistributedCache>();
        _repository = new CachedCommandsRepository<Project>(_innerRepositoryMock.Object, _cacheMock.Object);
    }

    [Fact]
    public async Task AddAsync_CallsInnerRepositoryAndInvalidatesCache()
    {
        // Arrange
        var project = new Project
        {
            Id = Guid.NewGuid(), 
            Title = "Test Project",
            Description = "Test Description"
        };

        // Act
        await _repository.AddAsync(project);

        // Assert
        _innerRepositoryMock.Verify(r => r.AddAsync(project, It.IsAny<CancellationToken>()), Times.Once());
        _cacheMock.Verify(c => c.RemoveAsync($"Project:{project.Id}", It.IsAny<CancellationToken>()), Times.Once());
        _cacheMock.Verify(c => c.RemoveAsync("Project:ListAll", It.IsAny<CancellationToken>()), Times.Once());
        _cacheMock.Verify(c => c.RemoveAsync("Project:CountAll", It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task UpdateAsync_CallsInnerRepositoryAndInvalidatesCache()
    {
        // Arrange
        var project = new Project
        {
            Id = Guid.NewGuid(), 
            Title = "Test Project",
            Description = "Test Description"
        };

        // Act
        await _repository.UpdateAsync(project);

        // Assert
        _innerRepositoryMock.Verify(r => r.UpdateAsync(project, It.IsAny<CancellationToken>()), Times.Once());
        _cacheMock.Verify(c => c.RemoveAsync($"Project:{project.Id}", It.IsAny<CancellationToken>()), Times.Once());
        _cacheMock.Verify(c => c.RemoveAsync("Project:ListAll", It.IsAny<CancellationToken>()), Times.Once());
        _cacheMock.Verify(c => c.RemoveAsync("Project:CountAll", It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task DeleteAsync_CallsInnerRepositoryAndInvalidatesCache()
    {
        // Arrange
        var project = new Project
        {
            Id = Guid.NewGuid(), 
            Title = "Test Project",
            Description = "Test Description"
        };

        // Act
        await _repository.DeleteAsync(project);

        // Assert
        _innerRepositoryMock.Verify(r => r.DeleteAsync(project, It.IsAny<CancellationToken>()), Times.Once());
        _cacheMock.Verify(c => c.RemoveAsync($"Project:{project.Id}", It.IsAny<CancellationToken>()), Times.Once());
        _cacheMock.Verify(c => c.RemoveAsync("Project:ListAll", It.IsAny<CancellationToken>()), Times.Once());
        _cacheMock.Verify(c => c.RemoveAsync("Project:CountAll", It.IsAny<CancellationToken>()), Times.Once());
    }
}
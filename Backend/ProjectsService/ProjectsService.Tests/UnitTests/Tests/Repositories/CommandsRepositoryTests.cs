using Microsoft.EntityFrameworkCore;
using ProjectsService.Infrastructure.Data;
using ProjectsService.Infrastructure.Repositories;

namespace ProjectsService.Tests.UnitTests.Tests.Repositories;

public class CommandsRepositoryTests
{
    private readonly CommandsDbContext _context;
    private readonly CommandsRepository<Project> _repository;

    public CommandsRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<CommandsDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        
        _context = new CommandsDbContext(options);
        _repository = new CommandsRepository<Project>(_context);
    }

    [Fact]
    public async Task AddAsync_AddsEntityToContext()
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
        await _context.SaveChangesAsync();

        // Assert
        var savedProject = await _context.Set<Project>().FindAsync(project.Id);
        savedProject.Should().BeEquivalentTo(project);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesEntityInContext()
    {
        // Arrange
        var project = new Project
        {
            Id = Guid.NewGuid(), 
            Title = "Initial Project",
            Description = "Test Description"
        };
        await _context.Set<Project>().AddAsync(project);
        await _context.SaveChangesAsync();

        project.Title = "Updated Title";

        // Act
        await _repository.UpdateAsync(project);
        await _context.SaveChangesAsync();

        // Assert
        var updatedProject = await _context.Set<Project>().FindAsync(project.Id);
        updatedProject.Title.Should().Be("Updated Title");
    }

    [Fact]
    public async Task DeleteAsync_RemovesEntityFromContext()
    {
        // Arrange
        var project = new Project
        {
            Id = Guid.NewGuid(), 
            Title = "Test Project",
            Description = "Test Description"
        };
        await _context.Set<Project>().AddAsync(project);
        await _context.SaveChangesAsync();

        // Act
        await _repository.DeleteAsync(project);
        await _context.SaveChangesAsync();

        // Assert
        var deletedProject = await _context.Set<Project>().FindAsync(project.Id);
        deletedProject.Should().BeNull();
    }
}
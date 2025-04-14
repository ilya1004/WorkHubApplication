using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using ProjectsService.Application.Specifications;
using ProjectsService.Domain.Abstractions.Specification;
using ProjectsService.Infrastructure.Data;
using ProjectsService.Infrastructure.Repositories;

namespace ProjectsService.Tests.UnitTests.Tests.Repositories;

public class QueriesRepositoryTests
{
    private readonly QueriesDbContext _context;
    private readonly QueriesRepository<Project> _repository;

    public QueriesRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<QueriesDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        
        _context = new QueriesDbContext(options);
        _repository = new QueriesRepository<Project>(_context);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsEntity_WhenExists()
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
        var result = await _repository.GetByIdAsync(project.Id);

        // Assert
        result.Should().BeEquivalentTo(project);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenNotExists()
    {
        // Arrange
        var id = Guid.NewGuid();

        // Act
        var result = await _repository.GetByIdAsync(id);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task ListAllAsync_ReturnsAllEntities()
    {
        // Arrange
        var projects = new List<Project>
        {
            new() { Id = Guid.NewGuid(), Title = "Project 1", Description = "Description 1" },
            new() { Id = Guid.NewGuid(), Title = "Project 2", Description = "Description 2" }
        };
        await _context.Set<Project>().AddRangeAsync(projects);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.ListAllAsync();

        // Assert
        result.Should().BeEquivalentTo(projects);
    }

    [Fact]
    public async Task PaginatedListAllAsync_ReturnsPaginatedEntities()
    {
        // Arrange
        var projects = new List<Project>
        {
            new() { Id = Guid.NewGuid(), Title = "Project 1", Description = "Description 1" },
            new() { Id = Guid.NewGuid(), Title = "Project 2", Description = "Description 2" },
            new() { Id = Guid.NewGuid(), Title = "Project 3", Description = "Description 3" }
        };
        await _context.Set<Project>().AddRangeAsync(projects);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.PaginatedListAllAsync(1, 2);

        // Assert
        result.Should().HaveCount(2);
        result.Should().BeEquivalentTo(projects.Take(2));
    }

    [Fact]
    public async Task CountAllAsync_ReturnsCorrectCount()
    {
        // Arrange
        var projects = new List<Project>
        {
            new() { Id = Guid.NewGuid(), Title = "Project 1", Description = "Description 1" },
            new() { Id = Guid.NewGuid(), Title = "Project 2", Description = "Description 2" }
        };
        await _context.Set<Project>().AddRangeAsync(projects);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.CountAllAsync();

        // Assert
        result.Should().Be(2);
    }

    [Fact]
    public async Task GetByFilterAsync_ReturnsFilteredEntities()
    {
        // Arrange
        var projects = new List<Project>
        {
            new() { Id = Guid.NewGuid(), Title = "Test Project", Description = "Test Description" },
            new() { Id = Guid.NewGuid(), Title = "Other Project", Description = "Other Description" }
        };
        await _context.Set<Project>().AddRangeAsync(projects);
        await _context.SaveChangesAsync();

        var specification = new Mock<ISpecification<Project>>();
        specification.Setup(s => s.Criteria).Returns(p => p.Title == "Test Project");
        specification.Setup(s => s.IncludesExpression).Returns(new List<Expression<Func<Project, object>>>());
        specification.Setup(s => s.OrderByExpression).Returns((Expression<Func<Project, object>>?)null);
        specification.Setup(s => s.OrderByDescExpression).Returns((Expression<Func<Project, object>>?)null);
        specification.Setup(s => s.IsPaginationEnabled).Returns(false);
        specification.Setup(s => s.Skip).Returns((int?)null);
        specification.Setup(s => s.Take).Returns((int?)null);

        // Act
        var result = await _repository.GetByFilterAsync(specification.Object);

        // Assert
        result.Should().HaveCount(1);
        result.First().Title.Should().Be("Test Project");
    }
}
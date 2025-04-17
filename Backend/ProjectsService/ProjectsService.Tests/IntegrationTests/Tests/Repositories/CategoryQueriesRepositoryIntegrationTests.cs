using System.Linq.Expressions;
using Microsoft.Extensions.DependencyInjection;
using ProjectsService.Domain.Abstractions.Data;
using ProjectsService.Tests.IntegrationTests.Helpers;

namespace ProjectsService.Tests.IntegrationTests.Tests.Repositories;

public class CategoryQueriesRepositoryIntegrationTests(
    IntegrationTestsFixture fixture) : IClassFixture<IntegrationTestsFixture>
{
    [Fact]
    public async Task GetByIdAsync_ShouldReturnCategory()
    {
        // Arrange
        using var scope = fixture.Factory.Services.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var category = new Category 
        { 
            Id = Guid.NewGuid(), 
            Name = "Category 1", 
            NormalizedName = "CATEGORY_1" 
        };
        await unitOfWork.CategoryCommandsRepository.AddAsync(category);
        await unitOfWork.SaveAllAsync();

        // Act
        var retrievedCategory = await unitOfWork.CategoryQueriesRepository.GetByIdAsync(category.Id);

        // Assert
        Assert.NotNull(retrievedCategory);
        Assert.Equal(category.Id, retrievedCategory.Id);
        Assert.Equal(category.Name, retrievedCategory.Name);
        Assert.Equal(category.NormalizedName, retrievedCategory.NormalizedName);
    }

    [Fact]
    public async Task ListAllAsync_ShouldReturnAllCategories()
    {
        // Arrange
        using var scope = fixture.Factory.Services.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var categories = new List<Category>
        {
            new Category { Id = Guid.NewGuid(), Name = "Category 1", NormalizedName = "CATEGORY_1" },
            new Category { Id = Guid.NewGuid(), Name = "Category 2", NormalizedName = "CATEGORY_2" }
        };
        foreach (var category in categories)
        {
            await unitOfWork.CategoryCommandsRepository.AddAsync(category);
        }
        await unitOfWork.SaveAllAsync();

        // Act
        var result = await unitOfWork.CategoryQueriesRepository.ListAllAsync();

        // Assert
        Assert.All(categories, c => Assert.Contains(result, r => r.Id == c.Id && r.Name == c.Name));
    }
    
    [Fact]
    public async Task ListAsync_WithFilter_ShouldReturnFilteredCategories()
    {
        // Arrange
        using var scope = fixture.Factory.Services.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var categories = new List<Category>
        {
            new Category { Id = Guid.NewGuid(), Name = "Category A", NormalizedName = "CATEGORY_A" },
            new Category { Id = Guid.NewGuid(), Name = "Category B", NormalizedName = "CATEGORY_B" },
            new Category { Id = Guid.NewGuid(), Name = "Category A", NormalizedName = "CATEGORY_A" }
        };
        foreach (var category in categories)
        {
            await unitOfWork.CategoryCommandsRepository.AddAsync(category);
        }
        await unitOfWork.SaveAllAsync();

        Expression<Func<Category, bool>> filter = c => c.Name == "Category A";

        // Act
        var result = await unitOfWork.CategoryQueriesRepository.ListAsync(filter);

        // Assert
        Assert.Equal(6, result.Count);
        Assert.All(result, r => Assert.Equal("Category A", r.Name));
    }

    [Fact]
    public async Task PaginatedListAsync_WithFilter_ShouldReturnFilteredPage()
    {
        // Arrange
        using var scope = fixture.Factory.Services.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var categories = new List<Category>
        {
            new Category { Id = Guid.NewGuid(), Name = "Category A", NormalizedName = "CATEGORY_A" },
            new Category { Id = Guid.NewGuid(), Name = "Category B", NormalizedName = "CATEGORY_B" },
            new Category { Id = Guid.NewGuid(), Name = "Category A", NormalizedName = "CATEGORY_A" }
        };
        foreach (var category in categories)
        {
            await unitOfWork.CategoryCommandsRepository.AddAsync(category);
        }
        await unitOfWork.SaveAllAsync();

        Expression<Func<Category, bool>> filter = c => c.Name == "Category A";

        // Act
        var result = await unitOfWork.CategoryQueriesRepository.PaginatedListAsync(filter, offset: 0, limit: 1);

        // Assert
        Assert.Single(result);
        Assert.Equal("Category A", result[0].Name);
    }

    [Fact]
    public async Task FirstOrDefaultAsync_ShouldReturnMatchingCategory()
    {
        // Arrange
        using var scope = fixture.Factory.Services.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var category = new Category 
        { 
            Id = Guid.NewGuid(), 
            Name = "Unique Category", 
            NormalizedName = "UNIQUE_CATEGORY" 
        };
        await unitOfWork.CategoryCommandsRepository.AddAsync(category);
        await unitOfWork.SaveAllAsync();

        Expression<Func<Category, bool>> filter = c => c.Name == "Unique Category";

        // Act
        var result = await unitOfWork.CategoryQueriesRepository.FirstOrDefaultAsync(filter);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(category.Id, result.Id);
        Assert.Equal(category.Name, result.Name);
    }

    [Fact]
    public async Task AnyAsync_ShouldReturnTrueForExistingCategory()
    {
        // Arrange
        using var scope = fixture.Factory.Services.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var category = new Category 
        { 
            Id = Guid.NewGuid(), 
            Name = "Category 1", 
            NormalizedName = "CATEGORY_1" 
        };
        await unitOfWork.CategoryCommandsRepository.AddAsync(category);
        await unitOfWork.SaveAllAsync();

        Expression<Func<Category, bool>> filter = c => c.Name == "Category 1";

        // Act
        var result = await unitOfWork.CategoryQueriesRepository.AnyAsync(filter);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task CountAllAsync_ShouldReturnTotalCount()
    {
        // Arrange
        using var scope = fixture.Factory.Services.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var categories = new List<Category>
        {
            new Category { Id = Guid.NewGuid(), Name = "Category 1", NormalizedName = "CATEGORY_1" },
            new Category { Id = Guid.NewGuid(), Name = "Category 2", NormalizedName = "CATEGORY_2" }
        };
        foreach (var category in categories)
        {
            await unitOfWork.CategoryCommandsRepository.AddAsync(category);
        }
        await unitOfWork.SaveAllAsync();

        // Act
        var count = await unitOfWork.CategoryQueriesRepository.CountAllAsync();

        // Assert
        Assert.Equal(11, count);
    }

    [Fact]
    public async Task CountAsync_WithFilter_ShouldReturnFilteredCount()
    {
        // Arrange
        using var scope = fixture.Factory.Services.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var categories = new List<Category>
        {
            new Category { Id = Guid.NewGuid(), Name = "Category A", NormalizedName = "CATEGORY_A" },
            new Category { Id = Guid.NewGuid(), Name = "Category B", NormalizedName = "CATEGORY_B" },
            new Category { Id = Guid.NewGuid(), Name = "Category A", NormalizedName = "CATEGORY_A" }
        };
        foreach (var category in categories)
        {
            await unitOfWork.CategoryCommandsRepository.AddAsync(category);
        }
        await unitOfWork.SaveAllAsync();

        Expression<Func<Category, bool>> filter = c => c.Name == "Category A";

        // Act
        var count = await unitOfWork.CategoryQueriesRepository.CountAsync(filter);

        // Assert
        Assert.Equal(4, count);
    }
}
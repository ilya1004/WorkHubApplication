using Microsoft.Extensions.DependencyInjection;
using ProjectsService.Domain.Abstractions.Data;
using ProjectsService.Tests.IntegrationTests.Helpers;

namespace ProjectsService.Tests.IntegrationTests.Tests.Repositories;

public class CategoryCommandsRepositoryIntegrationTests(
    IntegrationTestsFixture fixture) : IClassFixture<IntegrationTestsFixture>
{
    [Fact]
    public async Task AddCategory_ShouldPersistToDatabase()
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

        // Act
        await unitOfWork.CategoryCommandsRepository.AddAsync(category);
        await unitOfWork.SaveAllAsync();

        // Assert
        var retrievedCategory = await unitOfWork.CategoryQueriesRepository.GetByIdAsync(category.Id);
        Assert.NotNull(retrievedCategory);
        Assert.Equal(category.Id, retrievedCategory.Id);
        Assert.Equal(category.Name, retrievedCategory.Name);
        Assert.Equal(category.NormalizedName, retrievedCategory.NormalizedName);
    }

    [Fact]
    public async Task UpdateCategory_ShouldUpdateExistingCategory()
    {
        // Arrange
        using var scope = fixture.Factory.Services.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var category = new Category 
        { 
            Id = Guid.NewGuid(), 
            Name = "Original Name", 
            NormalizedName = "ORIGINAL_NAME" 
        };
        await unitOfWork.CategoryCommandsRepository.AddAsync(category);
        await unitOfWork.SaveAllAsync();

        // Act
        category.Name = "Updated Name";
        category.NormalizedName = "UPDATED_NAME";
        await unitOfWork.CategoryCommandsRepository.UpdateAsync(category);
        await unitOfWork.SaveAllAsync();

        // Assert
        var updatedCategory = await unitOfWork.CategoryQueriesRepository.GetByIdAsync(category.Id);
        Assert.NotNull(updatedCategory);
        Assert.Equal(category.Id, updatedCategory.Id);
        Assert.Equal("Updated Name", updatedCategory.Name);
        Assert.Equal("UPDATED_NAME", updatedCategory.NormalizedName);
    }

    [Fact]
    public async Task DeleteCategory_ShouldRemoveFromDatabase()
    {
        // Arrange
        using var scope = fixture.Factory.Services.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var category = new Category 
        { 
            Id = Guid.NewGuid(), 
            Name = "Category to Delete", 
            NormalizedName = "CATEGORY_TO_DELETE" 
        };
        await unitOfWork.CategoryCommandsRepository.AddAsync(category);
        await unitOfWork.SaveAllAsync();

        // Act
        await unitOfWork.CategoryCommandsRepository.DeleteAsync(category);
        await unitOfWork.SaveAllAsync();

        // Assert
        var deletedCategory = await unitOfWork.CategoryQueriesRepository.GetByIdAsync(category.Id);
        Assert.Null(deletedCategory);
    }
}
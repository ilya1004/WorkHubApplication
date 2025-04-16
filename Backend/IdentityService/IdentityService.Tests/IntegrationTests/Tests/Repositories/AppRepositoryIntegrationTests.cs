using System.Linq.Expressions;
using System.Text.Json;
using IdentityService.DAL.Abstractions.Repositories;
using IdentityService.DAL.Data;
using IdentityService.Tests.IntegrationTests.Helpers;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityService.Tests.IntegrationTests.Tests.Repositories;

public class AppRepositoryIntegrationTests : IClassFixture<IntegrationTestsFixture>
{
    private readonly IntegrationTestsFixture _fixture;

    public AppRepositoryIntegrationTests(IntegrationTestsFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task AddAsync_ShouldPersistEmployerIndustry()
    {
        // Arrange
        using var scope = _fixture.Factory.Services.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var employerIndustry = new EmployerIndustry
        {
            Id = Guid.NewGuid(),
            Name = "Technology",
            NormalizedName = "TECHNOLOGY"
        };

        // Act
        await unitOfWork.EmployerIndustriesRepository.AddAsync(employerIndustry);
        await unitOfWork.SaveAllAsync();

        // Assert
        var retrievedIndustry = await unitOfWork.EmployerIndustriesRepository.GetByIdAsync(employerIndustry.Id);
        retrievedIndustry.Should().NotBeNull();
        retrievedIndustry!.Id.Should().Be(employerIndustry.Id);
        retrievedIndustry.Name.Should().Be("Technology");
        retrievedIndustry.NormalizedName.Should().Be("TECHNOLOGY");
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateExistingEmployerIndustry()
    {
        // Arrange
        using var scope = _fixture.Factory.Services.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var employerIndustry = new EmployerIndustry
        {
            Id = Guid.NewGuid(),
            Name = "Finance",
            NormalizedName = "FINANCE"
        };
        await unitOfWork.EmployerIndustriesRepository.AddAsync(employerIndustry);
        await unitOfWork.SaveAllAsync();

        // Act
        employerIndustry.Name = "Updated Finance";
        employerIndustry.NormalizedName = "UPDATED_FINANCE";
        await unitOfWork.EmployerIndustriesRepository.UpdateAsync(employerIndustry);
        await unitOfWork.SaveAllAsync();

        // Assert
        var updatedIndustry = await unitOfWork.EmployerIndustriesRepository.GetByIdAsync(employerIndustry.Id);
        updatedIndustry.Should().NotBeNull();
        updatedIndustry!.Name.Should().Be("Updated Finance");
        updatedIndustry.NormalizedName.Should().Be("UPDATED_FINANCE");
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveEmployerIndustry()
    {
        // Arrange
        using var scope = _fixture.Factory.Services.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var employerIndustry = new EmployerIndustry
        {
            Id = Guid.NewGuid(),
            Name = "Healthcare",
            NormalizedName = "HEALTHCARE"
        };
        await unitOfWork.EmployerIndustriesRepository.AddAsync(employerIndustry);
        await unitOfWork.SaveAllAsync();

        // Act
        await unitOfWork.EmployerIndustriesRepository.DeleteAsync(employerIndustry);
        await unitOfWork.SaveAllAsync();

        // Assert
        var deletedIndustry = await unitOfWork.EmployerIndustriesRepository.GetByIdAsync(employerIndustry.Id);
        deletedIndustry.Should().BeNull();
    }
    
    [Fact]
    public async Task ListAllAsync_ShouldReturnAllEmployerIndustries()
    {
        // Arrange
        using var scope = _fixture.Factory.Services.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var industries = new List<EmployerIndustry>
        {
            new EmployerIndustry { Id = Guid.NewGuid(), Name = "Retail", NormalizedName = "RETAIL" },
            new EmployerIndustry { Id = Guid.NewGuid(), Name = "Manufacturing", NormalizedName = "MANUFACTURING" }
        };
        foreach (var industry in industries)
        {
            await unitOfWork.EmployerIndustriesRepository.AddAsync(industry);
        }

        await unitOfWork.SaveAllAsync();

        // Act
        var result = await unitOfWork.EmployerIndustriesRepository.ListAllAsync();

        // Assert
        result.Should().HaveCount(9);
        result.Should().Contain(i => i.Name == "Retail" && i.NormalizedName == "RETAIL");
        result.Should().Contain(i => i.Name == "Manufacturing" && i.NormalizedName == "MANUFACTURING");
    }

    [Fact]
    public async Task ListAsync_WithFilter_ShouldReturnFilteredIndustries()
    {
        // Arrange
        using var scope = _fixture.Factory.Services.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var industries = new List<EmployerIndustry>
        {
            new EmployerIndustry { Id = Guid.NewGuid(), Name = "Tech A", NormalizedName = "TECH_A" },
            new EmployerIndustry { Id = Guid.NewGuid(), Name = "Tech B", NormalizedName = "TECH_B" },
            new EmployerIndustry { Id = Guid.NewGuid(), Name = "Finance", NormalizedName = "FINANCE" }
        };
        foreach (var industry in industries)
        {
            await unitOfWork.EmployerIndustriesRepository.AddAsync(industry);
        }

        await unitOfWork.SaveAllAsync();
        Expression<Func<EmployerIndustry, bool>> filter = i => i.Name.StartsWith("Tech");

        // Act
        var result = await unitOfWork.EmployerIndustriesRepository.ListAsync(filter);

        // Assert
        result.Should().HaveCount(3);
        result.Should().OnlyContain(i => i.Name.StartsWith("Tech"));
    }

    [Fact]
    public async Task PaginatedListAsync_ShouldReturnPaginatedIndustries()
    {
        // Arrange
        using var scope = _fixture.Factory.Services.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var industries = new List<EmployerIndustry>
        {
            new EmployerIndustry { Id = Guid.NewGuid(), Name = "Industry 1", NormalizedName = "INDUSTRY_1" },
            new EmployerIndustry { Id = Guid.NewGuid(), Name = "Industry 2", NormalizedName = "INDUSTRY_2" },
            new EmployerIndustry { Id = Guid.NewGuid(), Name = "Industry 3", NormalizedName = "INDUSTRY_3" }
        };
        foreach (var industry in industries)
        {
            await unitOfWork.EmployerIndustriesRepository.AddAsync(industry);
        }

        await unitOfWork.SaveAllAsync();

        // Act
        var result = await unitOfWork.EmployerIndustriesRepository.PaginatedListAsync(null, 1, 2);

        // Assert
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task CountAllAsync_ShouldReturnCorrectCount()
    {
        // Arrange
        using var scope = _fixture.Factory.Services.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var industries = new List<EmployerIndustry>
        {
            new EmployerIndustry { Id = Guid.NewGuid(), Name = "Industry A", NormalizedName = "INDUSTRY_A" },
            new EmployerIndustry { Id = Guid.NewGuid(), Name = "Industry B", NormalizedName = "INDUSTRY_B" }
        };
        foreach (var industry in industries)
        {
            await unitOfWork.EmployerIndustriesRepository.AddAsync(industry);
        }

        await unitOfWork.SaveAllAsync();

        // Act
        var count = await unitOfWork.EmployerIndustriesRepository.CountAllAsync();

        // Assert
        count.Should().Be(17);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldCacheEmployerIndustry()
    {
        // Arrange
        using var scope = _fixture.Factory.Services.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var distributedCache = scope.ServiceProvider.GetRequiredService<IDistributedCache>();
        var employerIndustry = new EmployerIndustry
        {
            Id = Guid.NewGuid(),
            Name = "Cached Industry",
            NormalizedName = "CACHED_INDUSTRY"
        };
        await unitOfWork.EmployerIndustriesRepository.AddAsync(employerIndustry);
        await unitOfWork.SaveAllAsync();

        // Act
        var result1 = await unitOfWork.EmployerIndustriesRepository.GetByIdAsync(employerIndustry.Id);
        var cacheKey = $"EmployerIndustry:{employerIndustry.Id}";
        var cachedData = await distributedCache.GetStringAsync(cacheKey);
        var result2 = await unitOfWork.EmployerIndustriesRepository.GetByIdAsync(employerIndustry.Id);

        // Assert
        result1.Should().NotBeNull();
        cachedData.Should().NotBeNull();
        var cachedIndustry = JsonSerializer.Deserialize<EmployerIndustry>(cachedData!);
        cachedIndustry.Should().NotBeNull();
        cachedIndustry!.Id.Should().Be(employerIndustry.Id);
        result2.Should().NotBeNull();
        result2!.Name.Should().Be("Cached Industry");
    }

    [Fact]
    public async Task UpdateAsync_ShouldInvalidateCache()
    {
        // Arrange
        using var scope = _fixture.Factory.Services.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var distributedCache = scope.ServiceProvider.GetRequiredService<IDistributedCache>();
        var employerIndustry = new EmployerIndustry
        {
            Id = Guid.NewGuid(),
            Name = "Original Industry",
            NormalizedName = "ORIGINAL_INDUSTRY"
        };
        await unitOfWork.EmployerIndustriesRepository.AddAsync(employerIndustry);
        await unitOfWork.SaveAllAsync();
        await unitOfWork.EmployerIndustriesRepository.GetByIdAsync(employerIndustry.Id); // Populate cache

        // Act
        employerIndustry.Name = "Updated Industry";
        employerIndustry.NormalizedName = "UPDATED_INDUSTRY";
        await unitOfWork.EmployerIndustriesRepository.UpdateAsync(employerIndustry);
        await unitOfWork.SaveAllAsync();
        var cacheKey = $"EmployerIndustry:{employerIndustry.Id}";
        var cachedData = await distributedCache.GetStringAsync(cacheKey);

        // Assert
        cachedData.Should().BeNull();
        var updatedIndustry = await unitOfWork.EmployerIndustriesRepository.GetByIdAsync(employerIndustry.Id);
        updatedIndustry!.Name.Should().Be("Updated Industry");
    }

    [Fact]
    public async Task ListAllAsync_ShouldCacheList()
    {
        // Arrange
        using var scope = _fixture.Factory.Services.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var distributedCache = scope.ServiceProvider.GetRequiredService<IDistributedCache>();
        var industries = new List<EmployerIndustry>
        {
            new EmployerIndustry { Id = Guid.NewGuid(), Name = "Industry X", NormalizedName = "INDUSTRY_X" },
            new EmployerIndustry { Id = Guid.NewGuid(), Name = "Industry Y", NormalizedName = "INDUSTRY_Y" }
        };
        foreach (var industry in industries)
        {
            await unitOfWork.EmployerIndustriesRepository.AddAsync(industry);
        }

        await unitOfWork.SaveAllAsync();

        // Act
        var result1 = await unitOfWork.EmployerIndustriesRepository.ListAllAsync();
        var cacheKey = "EmployerIndustry:ListAll";
        var cachedData = await distributedCache.GetStringAsync(cacheKey);
        var result2 = await unitOfWork.EmployerIndustriesRepository.ListAllAsync();

        // Assert
        result1.Should().HaveCount(11);
        cachedData.Should().NotBeNull();
        var cachedIndustries = JsonSerializer.Deserialize<List<EmployerIndustry>>(cachedData!);
        cachedIndustries.Should().HaveCount(11);
        result2.Should().HaveCount(11);
    }

    [Fact]
    public async Task AddAsync_ShouldInvalidateListCache()
    {
        // Arrange
        using var scope = _fixture.Factory.Services.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var distributedCache = scope.ServiceProvider.GetRequiredService<IDistributedCache>();
        var employerIndustry1 = new EmployerIndustry
        {
            Id = Guid.NewGuid(),
            Name = "Industry 1",
            NormalizedName = "INDUSTRY_1"
        };
        await unitOfWork.EmployerIndustriesRepository.AddAsync(employerIndustry1);
        await unitOfWork.SaveAllAsync();
        await unitOfWork.EmployerIndustriesRepository.ListAllAsync(); // Populate cache

        // Act
        var employerIndustry2 = new EmployerIndustry
        {
            Id = Guid.NewGuid(),
            Name = "Industry 2",
            NormalizedName = "INDUSTRY_2"
        };
        await unitOfWork.EmployerIndustriesRepository.AddAsync(employerIndustry2);
        await unitOfWork.SaveAllAsync();
        var cacheKey = "EmployerIndustry:ListAll";
        var cachedData = await distributedCache.GetStringAsync(cacheKey);

        // Assert
        cachedData.Should().BeNull();
        var industries = await unitOfWork.EmployerIndustriesRepository.ListAllAsync();
        industries.Should().HaveCount(19);
    }

    [Fact]
    public async Task CountAllAsync_ShouldCacheCount()
    {
        // Arrange
        using var scope = _fixture.Factory.Services.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var distributedCache = scope.ServiceProvider.GetRequiredService<IDistributedCache>();
        var industries = new List<EmployerIndustry>
        {
            new EmployerIndustry { Id = Guid.NewGuid(), Name = "Industry A", NormalizedName = "INDUSTRY_A" },
            new EmployerIndustry { Id = Guid.NewGuid(), Name = "Industry B", NormalizedName = "INDUSTRY_B" }
        };
        foreach (var industry in industries)
        {
            await unitOfWork.EmployerIndustriesRepository.AddAsync(industry);
        }

        await unitOfWork.SaveAllAsync();

        // Act
        var count1 = await unitOfWork.EmployerIndustriesRepository.CountAllAsync();
        var cacheKey = "EmployerIndustry:CountAll";
        var cachedData = await distributedCache.GetStringAsync(cacheKey);
        var count2 = await unitOfWork.EmployerIndustriesRepository.CountAllAsync();

        // Assert
        count1.Should().Be(25);
        cachedData.Should().NotBeNull();
        int.Parse(cachedData).Should().Be(25);
        count2.Should().Be(25);
    }
}
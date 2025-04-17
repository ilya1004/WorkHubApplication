using System.Linq.Expressions;
using System.Text.Json;
using IdentityService.DAL.Abstractions.Repositories;
using IdentityService.DAL.Data;
using IdentityService.Tests.IntegrationTests.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityService.Tests.IntegrationTests.Tests.Repositories;

public class UsersRepositoryIntegrationTests : IClassFixture<IntegrationTestsFixture>
{
    private readonly IntegrationTestsFixture _fixture;

    public UsersRepositoryIntegrationTests(IntegrationTestsFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnAdminUser_WithoutTracking()
    {
        // Arrange
        using var scope = _fixture.Factory.Services.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        // Act
        var retrievedUser = await unitOfWork.UsersRepository.GetByIdAsync(
            (await scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>().FindByNameAsync("Admin"))!.Id,
            withTracking: false
        );

        // Assert
        retrievedUser.Should().NotBeNull();
        retrievedUser!.UserName.Should().Be("Admin");
        retrievedUser.Email.Should().Be("admin@gmail.com");
    }

    [Fact]
    public async Task GetByIdAsync_WithTrackingAndIncludes_ShouldIncludeFreelancerProfile()
    {
        // Arrange
        using var scope = _fixture.Factory.Services.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        // Act
        var retrievedUser = await unitOfWork.UsersRepository.GetByIdAsync(
            IntegrationTestsFixture.FreelancerId,
            withTracking: true,
            It.IsAny<CancellationToken>(),
            u => u.FreelancerProfile,
            u => u.FreelancerProfile!.Skills,
            u => u.Role
        );

        // Assert
        retrievedUser.Should().NotBeNull();
        retrievedUser!.UserName.Should().Be("Moonlight");
        retrievedUser.FreelancerProfile.Should().NotBeNull();
        retrievedUser.FreelancerProfile!.FirstName.Should().Be("Ilya");
        retrievedUser.FreelancerProfile.Skills.Should().HaveCount(4);
        retrievedUser.Role.Should().NotBeNull();
        retrievedUser.Role!.Name.Should().Be("Freelancer");
    }

    [Fact]
    public async Task FirstOrDefaultAsync_WithFilterAndIncludes_ShouldReturnEmployerUser()
    {
        // Arrange
        using var scope = _fixture.Factory.Services.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        Expression<Func<AppUser, bool>> filter = u => u.UserName == "Pavlusha";

        // Act
        var retrievedUser = await unitOfWork.UsersRepository.FirstOrDefaultAsync(
            filter,
            default,
            u => u.EmployerProfile,
            u => u.EmployerProfile!.Industry
        );

        // Assert
        retrievedUser.Should().NotBeNull();
        retrievedUser!.UserName.Should().Be("Pavlusha");
        retrievedUser.EmployerProfile.Should().NotBeNull();
        retrievedUser.EmployerProfile!.CompanyName.Should().Be("Sunrise Company");
        retrievedUser.EmployerProfile.Industry.Should().NotBeNull();
        retrievedUser.EmployerProfile.Industry!.Name.Should().Be("IT & Software");
    }

    [Fact]
    public async Task PaginatedListAllAsync_ShouldReturnPaginatedUsers()
    {
        // Arrange
        using var scope = _fixture.Factory.Services.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        // Act
        var result = await unitOfWork.UsersRepository.PaginatedListAllAsync(0, 2);

        // Assert
        result.Should().HaveCountGreaterThanOrEqualTo(2);
        result.Should().Contain(u => u.UserName == "Admin");
        result.Should().Contain(u => u.UserName == "Moonlight");
    }

    [Fact]
    public async Task PaginatedListAsync_WithFilter_ShouldReturnFilteredPaginatedUsers()
    {
        // Arrange
        using var scope = _fixture.Factory.Services.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        Expression<Func<AppUser, bool>> filter = u => u.UserName!.Contains("a");

        // Act
        var result = await unitOfWork.UsersRepository.PaginatedListAsync(filter, 0, 2);

        // Assert
        result.Should().HaveCountGreaterThanOrEqualTo(1);
        result.Should().Contain(u => u.UserName == "Pavlusha");
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateFreelancerUser()
    {
        // Arrange
        using var scope = _fixture.Factory.Services.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
        var user = await userManager.FindByNameAsync("Moonlight");

        // Act
        user!.Email = "updated.ilya@gmail.com";
        user.NormalizedEmail = "UPDATED.ILYA@GMAIL.COM";
        await unitOfWork.UsersRepository.UpdateAsync(user);
        await unitOfWork.SaveAllAsync();

        // Assert
        var updatedUser = await unitOfWork.UsersRepository.GetByIdAsync(IntegrationTestsFixture.FreelancerId);
        updatedUser.Should().NotBeNull();
        updatedUser!.Email.Should().Be("updated.ilya@gmail.com");
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveUser()
    {
        // Arrange
        using var scope = _fixture.Factory.Services.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
        var user = new AppUser
        {
            Id = Guid.NewGuid(),
            UserName = "todelete",
            NormalizedUserName = "TODELETE",
            Email = "todelete@example.com",
            NormalizedEmail = "TODELETE@EXAMPLE.COM",
            RegisteredAt = DateTime.UtcNow,
            EmailConfirmed = true,
            RoleId = (await scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>().FindByNameAsync("Freelancer"))!.Id
        };
        await userManager.CreateAsync(user, "ToDelete_123");
        await unitOfWork.SaveAllAsync();

        // Act
        await unitOfWork.UsersRepository.DeleteAsync(user);
        await unitOfWork.SaveAllAsync();

        // Assert
        var deletedUser = await unitOfWork.UsersRepository.GetByIdAsync(user.Id);
        deletedUser.Should().BeNull();
    }

    [Fact]
    public async Task CountAsync_WithFilter_ShouldReturnCorrectCount()
    {
        // Arrange
        using var scope = _fixture.Factory.Services.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        Expression<Func<AppUser, bool>> filter = u => u.Role!.Name == "Freelancer";

        // Act
        var count = await unitOfWork.UsersRepository.CountAsync(filter);

        // Assert
        count.Should().BeGreaterThanOrEqualTo(1);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldCacheEmployerUser()
    {
        // Arrange
        using var scope = _fixture.Factory.Services.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var distributedCache = scope.ServiceProvider.GetRequiredService<IDistributedCache>();

        // Act
        var result1 = await unitOfWork.UsersRepository.GetByIdAsync(IntegrationTestsFixture.EmployerId, withTracking: false);
        var cacheKey = $"AppUser:{IntegrationTestsFixture.EmployerId}";
        var cachedData = await distributedCache.GetStringAsync(cacheKey);
        var result2 = await unitOfWork.UsersRepository.GetByIdAsync(IntegrationTestsFixture.EmployerId, withTracking: false);

        // Assert
        result1.Should().NotBeNull();
        cachedData.Should().NotBeNull();
        var cachedUser = JsonSerializer.Deserialize<AppUser>(cachedData!);
        cachedUser.Should().NotBeNull();
        cachedUser!.Id.Should().Be(IntegrationTestsFixture.EmployerId);
        result2.Should().NotBeNull();
        result2!.UserName.Should().Be("Pavlusha");
    }

    [Fact]
    public async Task UpdateAsync_ShouldInvalidateCache()
    {
        // Arrange
        using var scope = _fixture.Factory.Services.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var distributedCache = scope.ServiceProvider.GetRequiredService<IDistributedCache>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
        var user = await userManager.FindByNameAsync("Pavlusha");
        await unitOfWork.UsersRepository.GetByIdAsync(IntegrationTestsFixture.EmployerId, withTracking: false);

        // Act
        user!.Email = "updated.pavlusha@gmail.com";
        await unitOfWork.UsersRepository.UpdateAsync(user);
        await unitOfWork.SaveAllAsync();
        var cacheKey = $"AppUser:{IntegrationTestsFixture.EmployerId}";
        var cachedData = await distributedCache.GetStringAsync(cacheKey);

        // Assert
        cachedData.Should().BeNull();
        var updatedUser = await unitOfWork.UsersRepository.GetByIdAsync(IntegrationTestsFixture.EmployerId, withTracking: false);
        updatedUser!.Email.Should().Be("updated.pavlusha@gmail.com");
    }

    [Fact]
    public async Task DeleteAsync_ShouldInvalidateCache()
    {
        // Arrange
        using var scope = _fixture.Factory.Services.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var distributedCache = scope.ServiceProvider.GetRequiredService<IDistributedCache>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
        var user = new AppUser
        {
            Id = Guid.NewGuid(),
            UserName = "todeletecache",
            NormalizedUserName = "TODELETECACHE",
            Email = "todeletecache@example.com",
            NormalizedEmail = "TODELETECACHE@EXAMPLE.COM",
            RegisteredAt = DateTime.UtcNow,
            EmailConfirmed = true,
            RoleId = (await scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>().FindByNameAsync("Freelancer"))!.Id
        };
        await userManager.CreateAsync(user, "ToDeleteCache_123");
        await unitOfWork.UsersRepository.GetByIdAsync(user.Id, withTracking: false);
        await unitOfWork.SaveAllAsync();

        // Act
        await unitOfWork.UsersRepository.DeleteAsync(user);
        await unitOfWork.SaveAllAsync();
        var cacheKey = $"AppUser:{user.Id}";
        var cachedData = await distributedCache.GetStringAsync(cacheKey);

        // Assert
        cachedData.Should().BeNull();
        var deletedUser = await unitOfWork.UsersRepository.GetByIdAsync(user.Id);
        deletedUser.Should().BeNull();
    }

    [Fact]
    public async Task GetByIdAsync_WithIncludes_ShouldNotCache()
    {
        // Arrange
        using var scope = _fixture.Factory.Services.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var distributedCache = scope.ServiceProvider.GetRequiredService<IDistributedCache>();

        // Act
        var result = await unitOfWork.UsersRepository.GetByIdAsync(
            IntegrationTestsFixture.EmployerId,
            withTracking: false,
            It.IsAny<CancellationToken>(),
            u => u.EmployerProfile
        );
        var cacheKey = $"AppUser:{IntegrationTestsFixture.EmployerId}";
        var cachedData = await distributedCache.GetStringAsync(cacheKey);

        // Assert
        result.Should().NotBeNull();
        result!.EmployerProfile.Should().NotBeNull();
        cachedData.Should().BeNull();
    }
}
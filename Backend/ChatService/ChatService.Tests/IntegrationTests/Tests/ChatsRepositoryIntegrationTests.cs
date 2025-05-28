using ChatService.Domain.Abstractions.Repositories;
using ChatService.Tests.IntegrationTests.Helpers;
using ChatService.Tests.UnitTests.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace ChatService.Tests.IntegrationTests.Tests;

public class ChatsRepositoryIntegrationTests(
    IntegrationTestsFixture fixture) : IClassFixture<IntegrationTestsFixture>
{
    [Fact]
    public async Task InsertAsync_ShouldAddChat()
    {
        // Arrange
        using var scope = fixture.Factory.Services.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var chat = new Chat
        {
            Id = Guid.NewGuid(),
            EmployerId = Guid.NewGuid(),
            FreelancerId = Guid.NewGuid(),
            ProjectId = Guid.NewGuid(),
            IsActive = true,
            CreatedAt = DateTime.UtcNow.TruncateToMilliseconds()
        };

        // Act
        await unitOfWork.ChatRepository.InsertAsync(chat);

        // Assert
        var result = await unitOfWork.ChatRepository.GetByIdAsync(chat.Id);
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(chat);
    }

    [Fact]
    public async Task ReplaceAsync_ShouldUpdateChat()
    {
        // Arrange
        using var scope = fixture.Factory.Services.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var chat = new Chat
        {
            Id = Guid.NewGuid(),
            EmployerId = Guid.NewGuid(),
            FreelancerId = Guid.NewGuid(),
            ProjectId = Guid.NewGuid(),
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        await unitOfWork.ChatRepository.InsertAsync(chat);

        var updatedChat = new Chat
        {
            Id = chat.Id,
            EmployerId = chat.EmployerId,
            FreelancerId = chat.FreelancerId,
            ProjectId = chat.ProjectId,
            IsActive = false,
            CreatedAt = chat.CreatedAt
        };

        // Act
        await unitOfWork.ChatRepository.ReplaceAsync(updatedChat);

        // Assert
        var result = await unitOfWork.ChatRepository.GetByIdAsync(chat.Id);
        result.Should().NotBeNull();
        result.IsActive.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveChat()
    {
        // Arrange
        using var scope = fixture.Factory.Services.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var chat = new Chat
        {
            Id = Guid.NewGuid(),
            EmployerId = Guid.NewGuid(),
            FreelancerId = Guid.NewGuid(),
            ProjectId = Guid.NewGuid(),
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        await unitOfWork.ChatRepository.InsertAsync(chat);

        // Act
        await unitOfWork.ChatRepository.DeleteAsync(chat.Id);

        // Assert
        var result = await unitOfWork.ChatRepository.GetByIdAsync(chat.Id);
        result.Should().BeNull();
    }

    [Fact]
    public async Task CountAllAsync_ShouldReturnCorrectCount()
    {
        // Arrange
        using var scope = fixture.Factory.Services.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var chats = new[]
        {
            new Chat
            {
                Id = Guid.NewGuid(), EmployerId = Guid.NewGuid(), FreelancerId = Guid.NewGuid(), ProjectId = Guid.NewGuid(), IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new Chat
            {
                Id = Guid.NewGuid(), EmployerId = Guid.NewGuid(), FreelancerId = Guid.NewGuid(), ProjectId = Guid.NewGuid(), IsActive = true,
                CreatedAt = DateTime.UtcNow
            }
        };
        foreach (var chat in chats)
        {
            await unitOfWork.ChatRepository.InsertAsync(chat);
        }

        // Act
        var count = await unitOfWork.ChatRepository.CountAllAsync();

        // Assert
        count.Should().Be(7);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnChat_WhenExists()
    {
        // Arrange
        using var scope = fixture.Factory.Services.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var chat = new Chat
        {
            Id = Guid.NewGuid(),
            EmployerId = Guid.NewGuid(),
            FreelancerId = Guid.NewGuid(),
            ProjectId = Guid.NewGuid(),
            IsActive = true,
            CreatedAt = DateTime.UtcNow.TruncateToMilliseconds()
        };
        await unitOfWork.ChatRepository.InsertAsync(chat);

        // Act
        var result = await unitOfWork.ChatRepository.GetByIdAsync(chat.Id);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(chat);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenNotExists()
    {
        // Arrange
        using var scope = fixture.Factory.Services.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        // Act
        var result = await unitOfWork.ChatRepository.GetByIdAsync(Guid.NewGuid());

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task FirstOrDefaultAsync_ShouldReturnChat_WhenMatchesFilter()
    {
        // Arrange
        using var scope = fixture.Factory.Services.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var employerId = Guid.NewGuid();
        var chat = new Chat
        {
            Id = Guid.NewGuid(),
            EmployerId = employerId,
            FreelancerId = Guid.NewGuid(),
            ProjectId = Guid.NewGuid(),
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        await unitOfWork.ChatRepository.InsertAsync(chat);

        // Act
        var result = await unitOfWork.ChatRepository.FirstOrDefaultAsync(c => c.EmployerId == employerId);

        // Assert
        result.Should().NotBeNull();
        result.EmployerId.Should().Be(employerId);
    }

    [Fact]
    public async Task FirstOrDefaultAsync_ShouldReturnNull_WhenNoMatch()
    {
        // Arrange
        using var scope = fixture.Factory.Services.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        // Act
        var result = await unitOfWork.ChatRepository.FirstOrDefaultAsync(c => c.EmployerId == Guid.NewGuid());

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task PaginatedListAllAsync_ShouldReturnPaginatedChats()
    {
        // Arrange
        using var scope = fixture.Factory.Services.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var chats = new[]
        {
            new Chat
            {
                Id = Guid.NewGuid(), EmployerId = Guid.NewGuid(), FreelancerId = Guid.NewGuid(), ProjectId = Guid.NewGuid(), IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new Chat
            {
                Id = Guid.NewGuid(), EmployerId = Guid.NewGuid(), FreelancerId = Guid.NewGuid(), ProjectId = Guid.NewGuid(), IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new Chat
            {
                Id = Guid.NewGuid(), EmployerId = Guid.NewGuid(), FreelancerId = Guid.NewGuid(), ProjectId = Guid.NewGuid(), IsActive = true,
                CreatedAt = DateTime.UtcNow
            }
        };
        foreach (var chat in chats)
        {
            await unitOfWork.ChatRepository.InsertAsync(chat);
        }

        // Act
        var result = await unitOfWork.ChatRepository.PaginatedListAllAsync(offset: 1, limit: 2);

        // Assert
        result.Should().HaveCount(2);
        result.Select(c => c.Id).Should().Contain(new[] { chats[1].Id, chats[2].Id });
    }
}
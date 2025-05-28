using ChatService.Domain.Abstractions.Repositories;
using ChatService.Tests.IntegrationTests.Helpers;
using ChatService.Tests.UnitTests.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace ChatService.Tests.IntegrationTests.Tests;

public class MessagesRepositoryIntegrationTests(
    IntegrationTestsFixture fixture) : IClassFixture<IntegrationTestsFixture>
{
    [Fact]
    public async Task InsertAsync_ShouldAddMessage()
    {
        // Arrange
        using var scope = fixture.Factory.Services.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var message = new Message
        {
            Id = Guid.NewGuid(),
            Text = "Hello!",
            ChatId = Guid.NewGuid(),
            SenderId = Guid.NewGuid(),
            ReceiverId = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow.TruncateToMilliseconds(),
            Type = MessageType.Text
        };

        // Act
        await unitOfWork.MessagesRepository.InsertAsync(message);

        // Assert
        var result = await unitOfWork.MessagesRepository.GetByIdAsync(message.Id);
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(message);
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveMessage()
    {
        // Arrange
        using var scope = fixture.Factory.Services.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var message = new Message
        {
            Id = Guid.NewGuid(),
            Text = "Hello!",
            ChatId = Guid.NewGuid(),
            SenderId = Guid.NewGuid(),
            ReceiverId = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            Type = MessageType.Text
        };
        await unitOfWork.MessagesRepository.InsertAsync(message);

        // Act
        await unitOfWork.MessagesRepository.DeleteAsync(message.Id);

        // Assert
        var result = await unitOfWork.MessagesRepository.GetByIdAsync(message.Id);
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnMessage_WhenExists()
    {
        // Arrange
        using var scope = fixture.Factory.Services.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var message = new Message
        {
            Id = Guid.NewGuid(),
            Text = "Hello!",
            ChatId = Guid.NewGuid(),
            SenderId = Guid.NewGuid(),
            ReceiverId = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow.TruncateToMilliseconds(),
            Type = MessageType.Text
        };
        await unitOfWork.MessagesRepository.InsertAsync(message);

        // Act
        var result = await unitOfWork.MessagesRepository.GetByIdAsync(message.Id);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(message);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenNotExists()
    {
        // Arrange
        using var scope = fixture.Factory.Services.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        // Act
        var result = await unitOfWork.MessagesRepository.GetByIdAsync(Guid.NewGuid());

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetMessagesByChatIdAsync_ShouldReturnPaginatedMessages()
    {
        // Arrange
        using var scope = fixture.Factory.Services.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var chatId = Guid.NewGuid();
        var messages = new[]
        {
            new Message
            {
                Id = Guid.NewGuid(), Text = "Message 1", ChatId = chatId, SenderId = Guid.NewGuid(), ReceiverId = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow.AddSeconds(-2), Type = MessageType.Text
            },
            new Message
            {
                Id = Guid.NewGuid(), Text = "Message 2", ChatId = chatId, SenderId = Guid.NewGuid(), ReceiverId = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow.AddSeconds(-1), Type = MessageType.Text
            },
            new Message
            {
                Id = Guid.NewGuid(), Text = "Message 3", ChatId = chatId, SenderId = Guid.NewGuid(), ReceiverId = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow, Type = MessageType.Text
            }
        };
        foreach (var message in messages)
        {
            await unitOfWork.MessagesRepository.InsertAsync(message);
        }

        // Act
        var result = await unitOfWork.MessagesRepository.GetMessagesByChatIdAsync(chatId, offset: 1, limit: 2);

        // Assert
        result.Should().HaveCount(2);
        result.Select(m => m.Text).Should().BeEquivalentTo(new[] { "Message 2", "Message 1" });
    }

    [Fact]
    public async Task CountAsync_ShouldReturnCorrectCount()
    {
        // Arrange
        using var scope = fixture.Factory.Services.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var chatId = Guid.NewGuid();
        var messages = new[]
        {
            new Message
            {
                Id = Guid.NewGuid(), Text = "Message 1", ChatId = chatId, SenderId = Guid.NewGuid(), ReceiverId = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow, Type = MessageType.Text
            },
            new Message
            {
                Id = Guid.NewGuid(), Text = "Message 2", ChatId = chatId, SenderId = Guid.NewGuid(), ReceiverId = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow, Type = MessageType.Text
            }
        };
        foreach (var message in messages)
        {
            await unitOfWork.MessagesRepository.InsertAsync(message);
        }

        // Act
        var count = await unitOfWork.MessagesRepository.CountAsync(m => m.ChatId == chatId);

        // Assert
        count.Should().Be(2);
    }
}    
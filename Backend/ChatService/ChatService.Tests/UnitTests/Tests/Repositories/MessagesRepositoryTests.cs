using System.Linq.Expressions;
using ChatService.Infrastructure.Constants;
using ChatService.Infrastructure.Repositories;
using ChatService.Tests.UnitTests.Extensions;
using MongoDB.Driver;

namespace ChatService.Tests.UnitTests.Tests.Repositories;

public class MessagesRepositoryTests : MongoTestBase
{
    private readonly MessagesRepository _repository;
    private readonly IMongoCollection<Message> _collection;

    public MessagesRepositoryTests()
    {
        _repository = new MessagesRepository(Database);
        _collection = Database.GetCollection<Message>(MongoDbCollections.Messages);
    }

    [Fact]
    public async Task InsertAsync_ShouldInsertMessage()
    {
        // Arrange
        var message = new Message
        {
            Id = Guid.NewGuid(),
            ChatId = Guid.NewGuid(),
            SenderId = Guid.NewGuid(),
            ReceiverId = Guid.NewGuid(),
            Text = "Hello",
            CreatedAt = DateTime.UtcNow.TruncateToMilliseconds(),
            Type = MessageType.Text
        };

        // Act
        await _repository.InsertAsync(message);

        // Assert
        var result = await _collection.Find(m => m.Id == message.Id).FirstOrDefaultAsync();
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(message);
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveMessage()
    {
        // Arrange
        var message = new Message { Id = Guid.NewGuid(), ChatId = Guid.NewGuid() };
        await _collection.InsertOneAsync(message);

        // Act
        await _repository.DeleteAsync(message.Id);

        // Assert
        var result = await _collection.Find(m => m.Id == message.Id).FirstOrDefaultAsync();
        result.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_WhenMessageDoesNotExist_ShouldNotThrow()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        await _repository.DeleteAsync(nonExistentId);

        // Assert
        var count = await _collection.CountDocumentsAsync(FilterDefinition<Message>.Empty);
        count.Should().Be(0);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnMessage_WhenExists()
    {
        // Arrange
        var message = new Message { Id = Guid.NewGuid(), ChatId = Guid.NewGuid() };
        await _collection.InsertOneAsync(message);

        // Act
        var result = await _repository.GetByIdAsync(message.Id);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(message);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenNotExists()
    {
        // Act
        var result = await _repository.GetByIdAsync(Guid.NewGuid());

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetMessagesByChatIdAsync_ShouldReturnMessages_SortedByCreatedAtDescending()
    {
        // Arrange
        var chatId = Guid.NewGuid();
        var messages = new[]
        {
            new Message { Id = Guid.NewGuid(), ChatId = chatId, CreatedAt = DateTime.UtcNow.AddMinutes(-2).TruncateToMilliseconds() },
            new Message { Id = Guid.NewGuid(), ChatId = chatId, CreatedAt = DateTime.UtcNow.AddMinutes(-1).TruncateToMilliseconds() },
            new Message { Id = Guid.NewGuid(), ChatId = chatId, CreatedAt = DateTime.UtcNow.TruncateToMilliseconds() }
        };
        await _collection.InsertManyAsync(messages);
        var offset = 1;
        var limit = 2;

        // Act
        var result = await _repository.GetMessagesByChatIdAsync(chatId, offset, limit);

        // Assert
        var expected = messages.OrderByDescending(m => m.CreatedAt).Skip(offset).Take(limit).ToList();
        result.Should().HaveCount(limit);
        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task GetMessagesByChatIdAsync_WhenNoMessages_ShouldReturnEmpty()
    {
        // Act
        var result = await _repository.GetMessagesByChatIdAsync(Guid.NewGuid(), offset: 0, limit: 5);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetMessagesByChatIdAsync_WhenOffsetExceedsCount_ShouldReturnEmpty()
    {
        // Arrange
        var chatId = Guid.NewGuid();
        var message = new Message { Id = Guid.NewGuid(), ChatId = chatId };
        await _collection.InsertOneAsync(message);

        // Act
        var result = await _repository.GetMessagesByChatIdAsync(chatId, offset: 10, limit: 5);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task CountAsync_ShouldReturnCorrectCount()
    {
        // Arrange
        var chatId = Guid.NewGuid();
        var messages = new[]
        {
            new Message { Id = Guid.NewGuid(), ChatId = chatId },
            new Message { Id = Guid.NewGuid(), ChatId = chatId }
        };
        await _collection.InsertManyAsync(messages);
        Expression<Func<Message, bool>> filter = m => m.ChatId == chatId;

        // Act
        var count = await _repository.CountAsync(filter);

        // Assert
        count.Should().Be(messages.Length);
    }

    [Fact]
    public async Task CountAsync_WhenNoMatches_ShouldReturnZero()
    {
        // Arrange
        Expression<Func<Message, bool>> filter = m => m.ChatId == Guid.NewGuid();

        // Act
        var count = await _repository.CountAsync(filter);

        // Assert
        count.Should().Be(0);
    }

    [Fact]
    public async Task InsertAsync_WithCancellationToken_ShouldCancel()
    {
        // Arrange
        var message = new Message { Id = Guid.NewGuid() };
        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act
        Func<Task> act = async () => await _repository.InsertAsync(message, cts.Token);

        // Assert
        await act.Should().ThrowAsync<OperationCanceledException>();
    }
}
using System.Linq.Expressions;
using ChatService.Infrastructure.Constants;
using ChatService.Infrastructure.Repositories;
using ChatService.Tests.UnitTests.Extensions;
using MongoDB.Driver;
using Xunit.Abstractions;

namespace ChatService.Tests.UnitTests.Tests.Repositories;

public class ChatsRepositoryTests : MongoTestBase
{
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly ChatsRepository _repository;
    private readonly IMongoCollection<Chat> _collection;

    public ChatsRepositoryTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        _repository = new ChatsRepository(Database);
        _collection = Database.GetCollection<Chat>(MongoDbCollections.Chats);
    }

    [Fact]
    public async Task InsertAsync_ShouldInsertChat()
    {
        // Arrange
        var chat = new Chat
        {
            Id = Guid.NewGuid(),
            EmployerId = Guid.NewGuid(),
            FreelancerId = Guid.NewGuid(),
            ProjectId = Guid.NewGuid(),
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        // Act
        await _repository.InsertAsync(chat);

        // Assert
        var result = await _collection.Find(c => c.Id == chat.Id).FirstOrDefaultAsync();
        result.Should().NotBeNull();
        result.Id.Should().Be(chat.Id);
        result.EmployerId.Should().Be(chat.EmployerId);
        result.FreelancerId.Should().Be(chat.FreelancerId);
        result.ProjectId.Should().Be(chat.ProjectId);
    }

    [Fact]
    public async Task ReplaceAsync_ShouldUpdateChat()
    {
        // Arrange
        var chat = new Chat
        {
            Id = Guid.NewGuid(),
            EmployerId = Guid.NewGuid(),
            FreelancerId = Guid.NewGuid(),
            ProjectId = Guid.NewGuid(),
            IsActive = true,
            CreatedAt = DateTime.UtcNow.TruncateToMilliseconds()
        };
        await _collection.InsertOneAsync(chat);
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
        await _repository.ReplaceAsync(updatedChat);

        // Assert
        var result = await _collection.Find(c => c.Id == chat.Id).FirstOrDefaultAsync();
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(updatedChat);
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveChat()
    {
        // Arrange
        var chat = new Chat { Id = Guid.NewGuid(), ProjectId = Guid.NewGuid() };
        await _collection.InsertOneAsync(chat);

        // Act
        await _repository.DeleteAsync(chat.Id);

        // Assert
        var result = await _collection.Find(c => c.Id == chat.Id).FirstOrDefaultAsync();
        result.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_WhenChatDoesNotExist_ShouldNotThrow()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        await _repository.DeleteAsync(nonExistentId);

        // Assert
        var count = await _collection.CountDocumentsAsync(FilterDefinition<Chat>.Empty);
        count.Should().Be(0);
    }

    [Fact]
    public async Task CountAllAsync_ShouldReturnCorrectCount()
    {
        // Arrange
        var chats = new[]
        {
            new Chat { Id = Guid.NewGuid(), ProjectId = Guid.NewGuid() },
            new Chat { Id = Guid.NewGuid(), ProjectId = Guid.NewGuid() }
        };
        await _collection.InsertManyAsync(chats);

        // Act
        var count = await _repository.CountAllAsync();

        // Assert
        count.Should().Be(chats.Length);
    }

    [Fact]
    public async Task CountAllAsync_WhenCollectionIsEmpty_ShouldReturnZero()
    {
        // Act
        var count = await _repository.CountAllAsync();

        // Assert
        count.Should().Be(0);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnChat_WhenExists()
    {
        // Arrange
        var chat = new Chat { Id = Guid.NewGuid(), ProjectId = Guid.NewGuid() };
        await _collection.InsertOneAsync(chat);

        // Act
        var result = await _repository.GetByIdAsync(chat.Id);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(chat);
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
    public async Task FirstOrDefaultAsync_ShouldReturnChat_WhenMatchesFilter()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var chat = new Chat { Id = Guid.NewGuid(), ProjectId = projectId };
        await _collection.InsertOneAsync(chat);
        Expression<Func<Chat, bool>> filter = c => c.ProjectId == projectId;

        // Act
        var result = await _repository.FirstOrDefaultAsync(filter);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(chat);
    }

    [Fact]
    public async Task FirstOrDefaultAsync_ShouldReturnNull_WhenNoMatch()
    {
        // Arrange
        Expression<Func<Chat, bool>> filter = c => c.ProjectId == Guid.NewGuid();

        // Act
        var result = await _repository.FirstOrDefaultAsync(filter);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task PaginatedListAllAsync_ShouldReturnCorrectPage()
    {
        // Arrange
        var chats = Enumerable.Range(1, 10)
            .Select(i => new Chat { Id = Guid.NewGuid(), ProjectId = Guid.NewGuid() })
            .ToList();
        await _collection.InsertManyAsync(chats);
        int offset = 2;
        int limit = 3;

        // Act
        var result = await _repository.PaginatedListAllAsync(offset, limit);

        // Assert
        result.Should().HaveCount(limit);
        result.Should().BeEquivalentTo(chats.Skip(offset).Take(limit));
    }

    [Fact]
    public async Task PaginatedListAllAsync_WhenOffsetExceedsCount_ShouldReturnEmpty()
    {
        // Arrange
        var chat = new Chat { Id = Guid.NewGuid(), ProjectId = Guid.NewGuid() };
        await _collection.InsertOneAsync(chat);

        // Act
        var result = await _repository.PaginatedListAllAsync(offset: 10, limit: 5);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task PaginatedListAllAsync_WhenCollectionIsEmpty_ShouldReturnEmpty()
    {
        // Act
        var result = await _repository.PaginatedListAllAsync(offset: 0, limit: 5);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task InsertAsync_WithCancellationToken_ShouldCancel()
    {
        // Arrange
        var chat = new Chat { Id = Guid.NewGuid() };
        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act
        Func<Task> act = async () => await _repository.InsertAsync(chat, cts.Token);

        // Assert
        await act.Should().ThrowAsync<OperationCanceledException>();
    }
}
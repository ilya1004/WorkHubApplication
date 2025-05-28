using System.Linq.Expressions;
using ChatService.Application.Exceptions;
using ChatService.Application.UseCases.MessageUseCases.Queries.GetChatMessages;
using ChatService.Domain.Abstractions.Repositories;
using ChatService.Domain.Abstractions.UserContext;
using ChatService.Tests.UnitTests.Extensions;

namespace ChatService.Tests.UnitTests.Tests.UseCases.MessageUseCases;

public class GetChatMessagesQueryHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IChatsRepository> _chatRepositoryMock = new();
    private readonly Mock<IMessagesRepository> _messagesRepositoryMock = new();
    private readonly Mock<IUserContext> _userContextMock = new();
    private readonly Mock<ILogger<GetChatMessagesQueryHandler>> _loggerMock = new();
    private readonly GetChatMessagesQueryHandler _handler;

    public GetChatMessagesQueryHandlerTests()
    {
        _unitOfWorkMock.Setup(u => u.ChatRepository).Returns(_chatRepositoryMock.Object);
        _unitOfWorkMock.Setup(u => u.MessagesRepository).Returns(_messagesRepositoryMock.Object);
        _handler = new GetChatMessagesQueryHandler(_unitOfWorkMock.Object, _userContextMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_WhenChatExistsAndUserAuthorized_ReturnsMessages()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var query = new GetChatMessagesQuery(ChatId: Guid.NewGuid(), PageNo: 2, PageSize: 10);
        var chat = new Chat { Id = query.ChatId, EmployerId = userId };
        var messages = new List<Message>
        {
            new Message { Id = Guid.NewGuid(), ChatId = query.ChatId },
            new Message { Id = Guid.NewGuid(), ChatId = query.ChatId }
        };
        var totalCount = 25;

        _userContextMock.Setup(u => u.GetUserId()).Returns(userId);
        _chatRepositoryMock.Setup(r => r.GetByIdAsync(query.ChatId, It.IsAny<CancellationToken>())).ReturnsAsync(chat);
        _messagesRepositoryMock
            .Setup(r => r.GetMessagesByChatIdAsync(query.ChatId, (query.PageNo - 1) * query.PageSize, query.PageSize, It.IsAny<CancellationToken>()))
            .ReturnsAsync(messages);
        _messagesRepositoryMock
            .Setup(r => r.CountAsync(It.Is<Expression<Func<Message, bool>>>(expr => expr.Compile()(new Message { ChatId = query.ChatId })), It.IsAny<CancellationToken>()))
            .ReturnsAsync(totalCount);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().BeEquivalentTo(messages);
        result.PageNo.Should().Be(query.PageNo);
        result.PageSize.Should().Be(query.PageSize);
        result.TotalCount.Should().Be(totalCount);
        _chatRepositoryMock.Verify(r => r.GetByIdAsync(query.ChatId, It.IsAny<CancellationToken>()), Times.Once());
        _messagesRepositoryMock.Verify(r => r.GetMessagesByChatIdAsync(query.ChatId, (query.PageNo - 1) * query.PageSize, query.PageSize, It.IsAny<CancellationToken>()), Times.Once());
        _messagesRepositoryMock.Verify(r => r.CountAsync(It.Is<Expression<Func<Message, bool>>>(expr => expr.Compile()(new Message { ChatId = query.ChatId })), It.IsAny<CancellationToken>()), Times.Once());
        LoggerMockExtensions.VerifyLog(_loggerMock, LogLevel.Information, $"Getting messages for chat {query.ChatId}. Page {query.PageNo}, Size {query.PageSize}", Times.Once());
        LoggerMockExtensions.VerifyLog(_loggerMock, LogLevel.Information, $"Retrieved {messages.Count} messages from chat {query.ChatId}", Times.Once());
    }

    [Fact]
    public async Task Handle_WhenChatDoesNotExist_ThrowsNotFoundException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var query = new GetChatMessagesQuery(Guid.NewGuid(), 1, 10);

        _userContextMock.Setup(u => u.GetUserId()).Returns(userId);
        _chatRepositoryMock.Setup(r => r.GetByIdAsync(query.ChatId, It.IsAny<CancellationToken>())).ReturnsAsync((Chat?)null);

        // Act
        Func<Task> act = async () => await _handler.Handle(query, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Chat with ID '{query.ChatId}' not found");
        _chatRepositoryMock.Verify(r => r.GetByIdAsync(query.ChatId, It.IsAny<CancellationToken>()), Times.Once());
        _messagesRepositoryMock.Verify(r => r.GetMessagesByChatIdAsync(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never());
        _messagesRepositoryMock.Verify(r => r.CountAsync(It.IsAny<Expression<Func<Message, bool>>>(), It.IsAny<CancellationToken>()), Times.Never());
        LoggerMockExtensions.VerifyLog(_loggerMock, LogLevel.Information, $"Getting messages for chat {query.ChatId}. Page {query.PageNo}, Size {query.PageSize}", Times.Once());
        LoggerMockExtensions.VerifyLog(_loggerMock, LogLevel.Warning, $"Chat {query.ChatId} not found", Times.Once());
    }

    [Fact]
    public async Task Handle_WhenUserNotAuthorized_ThrowsForbiddenException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var query = new GetChatMessagesQuery(Guid.NewGuid(), 1, 10);
        var chat = new Chat { Id = query.ChatId, EmployerId = Guid.NewGuid(), FreelancerId = Guid.NewGuid() };

        _userContextMock.Setup(u => u.GetUserId()).Returns(userId);
        _chatRepositoryMock.Setup(r => r.GetByIdAsync(query.ChatId, It.IsAny<CancellationToken>())).ReturnsAsync(chat);

        // Act
        Func<Task> act = async () => await _handler.Handle(query, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ForbiddenException>()
            .WithMessage($"You do not have access to chat with ID '{query.ChatId}'");
        _chatRepositoryMock.Verify(r => r.GetByIdAsync(query.ChatId, It.IsAny<CancellationToken>()), Times.Once());
        _messagesRepositoryMock.Verify(r => r.GetMessagesByChatIdAsync(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never());
        _messagesRepositoryMock.Verify(r => r.CountAsync(It.IsAny<Expression<Func<Message, bool>>>(), It.IsAny<CancellationToken>()), Times.Never());
        LoggerMockExtensions.VerifyLog(_loggerMock, LogLevel.Information, $"Getting messages for chat {query.ChatId}. Page {query.PageNo}, Size {query.PageSize}", Times.Once());
        LoggerMockExtensions.VerifyLog(_loggerMock, LogLevel.Warning, $"User {userId} has no access to chat {query.ChatId}", Times.Once());
    }
}
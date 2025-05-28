using ChatService.Application.Exceptions;
using ChatService.Application.UseCases.MessageUseCases.Commands.CreateTextMessage;
using ChatService.Domain.Abstractions.Repositories;
using ChatService.Domain.Abstractions.UserContext;
using ChatService.Tests.UnitTests.Extensions;

namespace ChatService.Tests.UnitTests.Tests.UseCases.MessageUseCases;

public class CreateTextMessageCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IChatsRepository> _chatRepositoryMock = new();
    private readonly Mock<IMessagesRepository> _messagesRepositoryMock = new();
    private readonly Mock<IUserContext> _userContextMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<ILogger<CreateTextMessageCommandHandler>> _loggerMock = new();
    private readonly CreateTextMessageCommandHandler _handler;

    public CreateTextMessageCommandHandlerTests()
    {
        _unitOfWorkMock.Setup(u => u.ChatRepository).Returns(_chatRepositoryMock.Object);
        _unitOfWorkMock.Setup(u => u.MessagesRepository).Returns(_messagesRepositoryMock.Object);
        _handler = new CreateTextMessageCommandHandler(
            _unitOfWorkMock.Object,
            _userContextMock.Object,
            _mapperMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_WhenChatExistsAndUserAuthorized_CreatesMessage()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new CreateTextMessageCommand(ChatId: Guid.NewGuid(), ReceiverId: Guid.NewGuid(), Text: "Hello");
        var chat = new Chat { Id = command.ChatId, EmployerId = userId };
        var message = new Message { Id = Guid.NewGuid(), ChatId = command.ChatId, ReceiverId = command.ReceiverId, Text = command.Text };

        _userContextMock.Setup(u => u.GetUserId()).Returns(userId);
        _chatRepositoryMock.Setup(r => r.GetByIdAsync(command.ChatId, It.IsAny<CancellationToken>())).ReturnsAsync(chat);
        _mapperMock.Setup(m => m.Map<Message>(command)).Returns(message);
        _messagesRepositoryMock.Setup(r => r.InsertAsync(message, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(message);
        message.SenderId.Should().Be(userId);
        _chatRepositoryMock.Verify(r => r.GetByIdAsync(command.ChatId, It.IsAny<CancellationToken>()), Times.Once());
        _mapperMock.Verify(m => m.Map<Message>(command), Times.Once());
        _messagesRepositoryMock.Verify(r => r.InsertAsync(message, It.IsAny<CancellationToken>()), Times.Once());
        LoggerMockExtensions.VerifyLog(_loggerMock, LogLevel.Information, $"Creating text message in chat {command.ChatId} by user {userId}", Times.Once());
        LoggerMockExtensions.VerifyLog(_loggerMock, LogLevel.Information, $"Text message created successfully. Message ID: {message.Id}", Times.Once());
    }

    [Fact]
    public async Task Handle_WhenChatDoesNotExist_ThrowsNotFoundException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new CreateTextMessageCommand(Guid.NewGuid(), Guid.NewGuid(), "Hello");

        _userContextMock.Setup(u => u.GetUserId()).Returns(userId);
        _chatRepositoryMock.Setup(r => r.GetByIdAsync(command.ChatId, It.IsAny<CancellationToken>())).ReturnsAsync((Chat?)null);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Chat with ID '{command.ChatId}' not found");
        _chatRepositoryMock.Verify(r => r.GetByIdAsync(command.ChatId, It.IsAny<CancellationToken>()), Times.Once());
        _mapperMock.Verify(m => m.Map<Message>(It.IsAny<CreateTextMessageCommand>()), Times.Never());
        _messagesRepositoryMock.Verify(r => r.InsertAsync(It.IsAny<Message>(), It.IsAny<CancellationToken>()), Times.Never());
        LoggerMockExtensions.VerifyLog(_loggerMock, LogLevel.Information, $"Creating text message in chat {command.ChatId} by user {userId}", Times.Once());
        LoggerMockExtensions.VerifyLog(_loggerMock, LogLevel.Warning, $"Chat {command.ChatId} not found", Times.Once());
    }

    [Fact]
    public async Task Handle_WhenUserNotAuthorized_ThrowsForbiddenException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new CreateTextMessageCommand(Guid.NewGuid(), Guid.NewGuid(), "Hello");
        var chat = new Chat { Id = command.ChatId, EmployerId = Guid.NewGuid(), FreelancerId = Guid.NewGuid() };

        _userContextMock.Setup(u => u.GetUserId()).Returns(userId);
        _chatRepositoryMock.Setup(r => r.GetByIdAsync(command.ChatId, It.IsAny<CancellationToken>())).ReturnsAsync(chat);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ForbiddenException>()
            .WithMessage($"You do not have access to chat with ID '{command.ChatId}'");
        _chatRepositoryMock.Verify(r => r.GetByIdAsync(command.ChatId, It.IsAny<CancellationToken>()), Times.Once());
        _mapperMock.Verify(m => m.Map<Message>(It.IsAny<CreateTextMessageCommand>()), Times.Never());
        _messagesRepositoryMock.Verify(r => r.InsertAsync(It.IsAny<Message>(), It.IsAny<CancellationToken>()), Times.Never());
        LoggerMockExtensions.VerifyLog(_loggerMock, LogLevel.Information, $"Creating text message in chat {command.ChatId} by user {userId}", Times.Once());
        LoggerMockExtensions.VerifyLog(_loggerMock, LogLevel.Warning, $"User {userId} has no access to chat {command.ChatId}", Times.Once());
    }
}
using ChatService.Application.Exceptions;
using ChatService.Application.UseCases.MessageUseCases.Commands.CreateFileMessage;
using ChatService.Domain.Abstractions.BlobService;
using ChatService.Domain.Abstractions.Repositories;
using ChatService.Domain.Abstractions.UserContext;
using ChatService.Tests.UnitTests.Extensions;

namespace ChatService.Tests.UnitTests.Tests.UseCases.MessageUseCases;

public class CreateFileMessageCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IChatsRepository> _chatRepositoryMock = new();
    private readonly Mock<IMessagesRepository> _messagesRepositoryMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<IUserContext> _userContextMock = new();
    private readonly Mock<IBlobService> _blobServiceMock = new();
    private readonly Mock<ILogger<CreateFileMessageCommandHandler>> _loggerMock = new();
    private readonly CreateFileMessageCommandHandler _handler;

    public CreateFileMessageCommandHandlerTests()
    {
        _unitOfWorkMock.Setup(u => u.ChatRepository).Returns(_chatRepositoryMock.Object);
        _unitOfWorkMock.Setup(u => u.MessagesRepository).Returns(_messagesRepositoryMock.Object);
        _handler = new CreateFileMessageCommandHandler(
            _unitOfWorkMock.Object,
            _mapperMock.Object,
            _userContextMock.Object,
            _blobServiceMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_WhenChatExistsAndUserAuthorized_CreatesMessageAndUploadsFile()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new CreateFileMessageCommand(
            ChatId: Guid.NewGuid(),
            ReceiverId: Guid.NewGuid(),
            FileStream: new MemoryStream(new byte[] { 1, 2, 3 }),
            ContentType: "application/pdf");
        var chat = new Chat { Id = command.ChatId, EmployerId = userId };
        var message = new Message { Id = Guid.NewGuid(), ChatId = command.ChatId, ReceiverId = command.ReceiverId };
        var fileId = Guid.NewGuid();

        _userContextMock.Setup(u => u.GetUserId()).Returns(userId);
        _chatRepositoryMock.Setup(r => r.GetByIdAsync(command.ChatId, It.IsAny<CancellationToken>())).ReturnsAsync(chat);
        _mapperMock.Setup(m => m.Map<Message>(command)).Returns(message);
        _blobServiceMock.Setup(b => b.UploadAsync(command.FileStream, command.ContentType, It.IsAny<CancellationToken>())).ReturnsAsync(fileId);
        _messagesRepositoryMock.Setup(r => r.InsertAsync(message, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(message);
        message.SenderId.Should().Be(userId);
        message.FileId.Should().Be(fileId);
        _chatRepositoryMock.Verify(r => r.GetByIdAsync(command.ChatId, It.IsAny<CancellationToken>()), Times.Once());
        _mapperMock.Verify(m => m.Map<Message>(command), Times.Once());
        _blobServiceMock.Verify(b => b.UploadAsync(command.FileStream, command.ContentType, It.IsAny<CancellationToken>()), Times.Once());
        _messagesRepositoryMock.Verify(r => r.InsertAsync(message, It.IsAny<CancellationToken>()), Times.Once());
        LoggerMockExtensions.VerifyLog(_loggerMock, LogLevel.Information, $"Creating file message in chat {command.ChatId} by user {userId}", Times.Once());
        LoggerMockExtensions.VerifyLog(_loggerMock, LogLevel.Information, $"File message created successfully. File ID: {fileId}, Message ID: {message.Id}", Times.Once());
    }

    [Fact]
    public async Task Handle_WhenChatDoesNotExist_ThrowsNotFoundException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new CreateFileMessageCommand(Guid.NewGuid(), Guid.NewGuid(), new MemoryStream(), "application/pdf");

        _userContextMock.Setup(u => u.GetUserId()).Returns(userId);
        _chatRepositoryMock.Setup(r => r.GetByIdAsync(command.ChatId, It.IsAny<CancellationToken>())).ReturnsAsync((Chat?)null);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Chat with ID '{command.ChatId}' not found");
        _chatRepositoryMock.Verify(r => r.GetByIdAsync(command.ChatId, It.IsAny<CancellationToken>()), Times.Once());
        _mapperMock.Verify(m => m.Map<Message>(It.IsAny<CreateFileMessageCommand>()), Times.Never());
        _blobServiceMock.Verify(b => b.UploadAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never());
        _messagesRepositoryMock.Verify(r => r.InsertAsync(It.IsAny<Message>(), It.IsAny<CancellationToken>()), Times.Never());
        LoggerMockExtensions.VerifyLog(_loggerMock, LogLevel.Information, $"Creating file message in chat {command.ChatId} by user {userId}", Times.Once());
        LoggerMockExtensions.VerifyLog(_loggerMock, LogLevel.Warning, $"Chat {command.ChatId} not found", Times.Once());
    }

    [Fact]
    public async Task Handle_WhenUserNotAuthorized_ThrowsForbiddenException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new CreateFileMessageCommand(Guid.NewGuid(), Guid.NewGuid(), new MemoryStream(), "application/pdf");
        var chat = new Chat { Id = command.ChatId, EmployerId = Guid.NewGuid(), FreelancerId = Guid.NewGuid() };

        _userContextMock.Setup(u => u.GetUserId()).Returns(userId);
        _chatRepositoryMock.Setup(r => r.GetByIdAsync(command.ChatId, It.IsAny<CancellationToken>())).ReturnsAsync(chat);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ForbiddenException>()
            .WithMessage($"You do not have access to chat with ID '{command.ChatId}'");
        _chatRepositoryMock.Verify(r => r.GetByIdAsync(command.ChatId, It.IsAny<CancellationToken>()), Times.Once());
        _mapperMock.Verify(m => m.Map<Message>(It.IsAny<CreateFileMessageCommand>()), Times.Never());
        _blobServiceMock.Verify(b => b.UploadAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never());
        _messagesRepositoryMock.Verify(r => r.InsertAsync(It.IsAny<Message>(), It.IsAny<CancellationToken>()), Times.Never());
        LoggerMockExtensions.VerifyLog(_loggerMock, LogLevel.Information, $"Creating file message in chat {command.ChatId} by user {userId}", Times.Once());
        LoggerMockExtensions.VerifyLog(_loggerMock, LogLevel.Warning, $"User {userId} has no access to chat {command.ChatId}", Times.Once());
    }
}
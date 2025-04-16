using ChatService.Application.Exceptions;
using ChatService.Application.UseCases.MessageUseCases.Commands.DeleteMessage;
using ChatService.Domain.Abstractions.BlobService;
using ChatService.Domain.Abstractions.Repositories;
using ChatService.Domain.Abstractions.UserContext;
using ChatService.Tests.UnitTests.Extensions;

namespace ChatService.Tests.UnitTests.Tests.UseCases.MessageUseCases;

public class DeleteMessageCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IMessagesRepository> _messagesRepositoryMock = new();
    private readonly Mock<IUserContext> _userContextMock = new();
    private readonly Mock<IBlobService> _blobServiceMock = new();
    private readonly Mock<ILogger<DeleteMessageCommandHandler>> _loggerMock = new();
    private readonly DeleteMessageCommandHandler _handler;

    public DeleteMessageCommandHandlerTests()
    {
        _unitOfWorkMock.Setup(u => u.MessagesRepository).Returns(_messagesRepositoryMock.Object);
        _handler = new DeleteMessageCommandHandler(
            _unitOfWorkMock.Object,
            _userContextMock.Object,
            _blobServiceMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_WhenMessageExistsAndUserIsSender_DeletesMessage()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new DeleteMessageCommand(Guid.NewGuid());
        var message = new Message { Id = command.MessageId, SenderId = userId, Type = MessageType.Text };

        _userContextMock.Setup(u => u.GetUserId()).Returns(userId);
        _messagesRepositoryMock.Setup(r => r.GetByIdAsync(command.MessageId, It.IsAny<CancellationToken>())).ReturnsAsync(message);
        _messagesRepositoryMock.Setup(r => r.DeleteAsync(command.MessageId, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _messagesRepositoryMock.Verify(r => r.GetByIdAsync(command.MessageId, It.IsAny<CancellationToken>()), Times.Once());
        _messagesRepositoryMock.Verify(r => r.DeleteAsync(command.MessageId, It.IsAny<CancellationToken>()), Times.Once());
        _blobServiceMock.Verify(b => b.DeleteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never());
        LoggerMockExtensions.VerifyLog(_loggerMock, LogLevel.Information, $"Deleting message {command.MessageId} by user {userId}", Times.Once());
        LoggerMockExtensions.VerifyLog(_loggerMock, LogLevel.Information, $"Message {command.MessageId} deleted successfully", Times.Once());
    }

    [Fact]
    public async Task Handle_WhenMessageIsFileAndUserIsSender_DeletesMessageAndFile()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var fileId = Guid.NewGuid();
        var command = new DeleteMessageCommand(Guid.NewGuid());
        var message = new Message { Id = command.MessageId, SenderId = userId, Type = MessageType.File, FileId = fileId };

        _userContextMock.Setup(u => u.GetUserId()).Returns(userId);
        _messagesRepositoryMock.Setup(r => r.GetByIdAsync(command.MessageId, It.IsAny<CancellationToken>())).ReturnsAsync(message);
        _blobServiceMock.Setup(b => b.DeleteAsync(fileId, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        _messagesRepositoryMock.Setup(r => r.DeleteAsync(command.MessageId, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _messagesRepositoryMock.Verify(r => r.GetByIdAsync(command.MessageId, It.IsAny<CancellationToken>()), Times.Once());
        _blobServiceMock.Verify(b => b.DeleteAsync(fileId, It.IsAny<CancellationToken>()), Times.Once());
        _messagesRepositoryMock.Verify(r => r.DeleteAsync(command.MessageId, It.IsAny<CancellationToken>()), Times.Once());
        LoggerMockExtensions.VerifyLog(_loggerMock, LogLevel.Information, $"Deleting message {command.MessageId} by user {userId}", Times.Once());
        LoggerMockExtensions.VerifyLog(_loggerMock, LogLevel.Information, $"Deleting file {fileId} from blob storage", Times.Once());
        LoggerMockExtensions.VerifyLog(_loggerMock, LogLevel.Information, $"Message {command.MessageId} deleted successfully", Times.Once());
    }

    [Fact]
    public async Task Handle_WhenMessageDoesNotExist_ThrowsNotFoundException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new DeleteMessageCommand(Guid.NewGuid());

        _userContextMock.Setup(u => u.GetUserId()).Returns(userId);
        _messagesRepositoryMock.Setup(r => r.GetByIdAsync(command.MessageId, It.IsAny<CancellationToken>())).ReturnsAsync((Message?)null);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Message with ID '{command.MessageId}' not found");
        _messagesRepositoryMock.Verify(r => r.GetByIdAsync(command.MessageId, It.IsAny<CancellationToken>()), Times.Once());
        _blobServiceMock.Verify(b => b.DeleteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never());
        _messagesRepositoryMock.Verify(r => r.DeleteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never());
        LoggerMockExtensions.VerifyLog(_loggerMock, LogLevel.Information, $"Deleting message {command.MessageId} by user {userId}", Times.Once());
        LoggerMockExtensions.VerifyLog(_loggerMock, LogLevel.Warning, $"Message {command.MessageId} not found", Times.Once());
    }

    [Fact]
    public async Task Handle_WhenUserIsNotSender_ThrowsForbiddenException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var senderId = Guid.NewGuid();
        var command = new DeleteMessageCommand(Guid.NewGuid());
        var message = new Message { Id = command.MessageId, SenderId = senderId, Type = MessageType.Text };

        _userContextMock.Setup(u => u.GetUserId()).Returns(userId);
        _messagesRepositoryMock.Setup(r => r.GetByIdAsync(command.MessageId, It.IsAny<CancellationToken>())).ReturnsAsync(message);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ForbiddenException>()
            .WithMessage($"You cannot delete message with ID '{command.MessageId}' which is not yours");
        _messagesRepositoryMock.Verify(r => r.GetByIdAsync(command.MessageId, It.IsAny<CancellationToken>()), Times.Once());
        _blobServiceMock.Verify(b => b.DeleteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never());
        _messagesRepositoryMock.Verify(r => r.DeleteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never());
        LoggerMockExtensions.VerifyLog(_loggerMock, LogLevel.Information, $"Deleting message {command.MessageId} by user {userId}", Times.Once());
        LoggerMockExtensions.VerifyLog(_loggerMock, LogLevel.Warning, $"User {userId} tried to delete message {command.MessageId} of user {senderId}", Times.Once());
    }
}
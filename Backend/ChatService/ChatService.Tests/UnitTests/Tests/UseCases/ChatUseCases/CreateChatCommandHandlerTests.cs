using System.Linq.Expressions;
using ChatService.Application.Exceptions;
using ChatService.Application.UseCases.ChatUseCases.Commands.CreateChat;
using ChatService.Domain.Abstractions.Repositories;
using ChatService.Tests.UnitTests.Extensions;

namespace ChatService.Tests.UnitTests.Tests.UseCases.ChatUseCases;

public class CreateChatCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IChatsRepository> _chatRepositoryMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<ILogger<CreateChatCommandHandler>> _loggerMock = new();
    private readonly CreateChatCommandHandler _handler;

    public CreateChatCommandHandlerTests()
    {
        _unitOfWorkMock.Setup(u => u.ChatRepository).Returns(_chatRepositoryMock.Object);
        _handler = new CreateChatCommandHandler(_unitOfWorkMock.Object, _mapperMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_WhenChatDoesNotExist_CreatesChatAndLogs()
    {
        // Arrange
        var command = new CreateChatCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        var newChat = new Chat { Id = Guid.NewGuid(), ProjectId = command.ProjectId };

        _chatRepositoryMock
            .Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<Chat, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Chat?)null);
        _mapperMock.Setup(m => m.Map<Chat>(command)).Returns(newChat);
        _chatRepositoryMock.Setup(r => r.InsertAsync(newChat, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _chatRepositoryMock.Verify(
            r => r.FirstOrDefaultAsync(
                It.Is<Expression<Func<Chat, bool>>>(expr => expr.Compile()(new Chat { ProjectId = command.ProjectId })),
                It.IsAny<CancellationToken>()),
            Times.Once());
        _mapperMock.Verify(m => m.Map<Chat>(command), Times.Once());
        _chatRepositoryMock.Verify(r => r.InsertAsync(newChat, It.IsAny<CancellationToken>()), Times.Once());
        LoggerMockExtensions.VerifyLog(_loggerMock, LogLevel.Information, $"Creating new chat for project {command.ProjectId}", Times.Once());
        LoggerMockExtensions.VerifyLog(_loggerMock, LogLevel.Information, $"Successfully created chat {newChat.Id} for project {command.ProjectId}", Times.Once());
    }

    [Fact]
    public async Task Handle_WhenChatExists_ThrowsAlreadyExistsException()
    {
        // Arrange
        var command = new CreateChatCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        var existingChat = new Chat { Id = Guid.NewGuid(), ProjectId = command.ProjectId };

        _chatRepositoryMock
            .Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<Chat, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingChat);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<AlreadyExistsException>()
            .WithMessage("Chat for this project already exists");
        _chatRepositoryMock.Verify(
            r => r.FirstOrDefaultAsync(
                It.Is<Expression<Func<Chat, bool>>>(expr => expr.Compile()(new Chat { ProjectId = command.ProjectId })),
                It.IsAny<CancellationToken>()),
            Times.Once());
        _mapperMock.Verify(m => m.Map<Chat>(It.IsAny<CreateChatCommand>()), Times.Never());
        _chatRepositoryMock.Verify(r => r.InsertAsync(It.IsAny<Chat>(), It.IsAny<CancellationToken>()), Times.Never());
        LoggerMockExtensions.VerifyLog(_loggerMock, LogLevel.Information, $"Creating new chat for project {command.ProjectId}", Times.Once());
        LoggerMockExtensions.VerifyLog(_loggerMock, LogLevel.Warning, $"Chat already exists for project {command.ProjectId}", Times.Once());
    }
}
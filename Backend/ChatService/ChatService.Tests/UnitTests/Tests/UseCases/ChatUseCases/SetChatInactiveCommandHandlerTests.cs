using System.Linq.Expressions;
using ChatService.Application.Exceptions;
using ChatService.Application.UseCases.ChatUseCases.Commands.SetChatInactive;
using ChatService.Domain.Abstractions.Repositories;
using ChatService.Tests.UnitTests.Extensions;

namespace ChatService.Tests.UnitTests.Tests.UseCases.ChatUseCases;

public class SetChatInactiveCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IChatsRepository> _chatRepositoryMock = new();
    private readonly Mock<ILogger<SetChatInactiveCommandHandler>> _loggerMock = new();
    private readonly SetChatInactiveCommandHandler _handler;

    public SetChatInactiveCommandHandlerTests()
    {
        _unitOfWorkMock.Setup(u => u.ChatRepository).Returns(_chatRepositoryMock.Object);
        _handler = new SetChatInactiveCommandHandler(_unitOfWorkMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_WhenChatExists_SetsInactiveAndLogs()
    {
        // Arrange
        var command = new SetChatInactiveCommand(Guid.NewGuid());
        var chat = new Chat { Id = Guid.NewGuid(), ProjectId = command.ProjectId, IsActive = true };

        _chatRepositoryMock
            .Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<Chat, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(chat);
        _chatRepositoryMock.Setup(r => r.ReplaceAsync(chat, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        chat.IsActive.Should().BeFalse();
        _chatRepositoryMock.Verify(
            r => r.FirstOrDefaultAsync(
                It.Is<Expression<Func<Chat, bool>>>(expr => expr.Compile()(new Chat { ProjectId = command.ProjectId })),
                It.IsAny<CancellationToken>()),
            Times.Once());
        _chatRepositoryMock.Verify(r => r.ReplaceAsync(chat, It.IsAny<CancellationToken>()), Times.Once());
        LoggerMockExtensions.VerifyLog(_loggerMock, LogLevel.Information, $"Setting chat by project ID '{command.ProjectId}' to inactive", Times.Once());
        LoggerMockExtensions.VerifyLog(_loggerMock, LogLevel.Information, $"Chat by project ID '{command.ProjectId}' successfully set to inactive", Times.Once());
    }

    [Fact]
    public async Task Handle_WhenChatDoesNotExist_ThrowsNotFoundException()
    {
        // Arrange
        var command = new SetChatInactiveCommand(Guid.NewGuid());

        _chatRepositoryMock
            .Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<Chat, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Chat?)null);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Chat with project ID '{command.ProjectId}' not found");
        _chatRepositoryMock.Verify(
            r => r.FirstOrDefaultAsync(
                It.Is<Expression<Func<Chat, bool>>>(expr => expr.Compile()(new Chat { ProjectId = command.ProjectId })),
                It.IsAny<CancellationToken>()),
            Times.Once());
        _chatRepositoryMock.Verify(r => r.ReplaceAsync(It.IsAny<Chat>(), It.IsAny<CancellationToken>()), Times.Never());
        LoggerMockExtensions.VerifyLog(_loggerMock, LogLevel.Information, $"Setting chat by project ID '{command.ProjectId}' to inactive", Times.Once());
        LoggerMockExtensions.VerifyLog(_loggerMock, LogLevel.Error, $"Chat with by project ID '{command.ProjectId}' not found", Times.Once());
    }
}
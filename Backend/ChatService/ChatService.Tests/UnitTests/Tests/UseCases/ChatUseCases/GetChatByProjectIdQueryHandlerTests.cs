using System.Linq.Expressions;
using ChatService.Application.UseCases.ChatUseCases.Queries.GetChatByProjectId;
using ChatService.Domain.Abstractions.Repositories;
using ChatService.Tests.UnitTests.Extensions;

namespace ChatService.Tests.UnitTests.Tests.UseCases.ChatUseCases;

public class GetChatByProjectIdQueryHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IChatsRepository> _chatRepositoryMock = new();
    private readonly Mock<ILogger<GetChatByProjectIdQueryHandler>> _loggerMock = new();
    private readonly GetChatByProjectIdQueryHandler _handler;

    public GetChatByProjectIdQueryHandlerTests()
    {
        _unitOfWorkMock.Setup(u => u.ChatRepository).Returns(_chatRepositoryMock.Object);
        _handler = new GetChatByProjectIdQueryHandler(_loggerMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_WhenChatExistsAndActive_ReturnsChatAndLogs()
    {
        // Arrange
        var query = new GetChatByProjectIdQuery(Guid.NewGuid());
        var chat = new Chat { Id = Guid.NewGuid(), ProjectId = query.ProjectId, IsActive = true };

        _chatRepositoryMock
            .Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<Chat, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(chat);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().Be(chat);
        _chatRepositoryMock.Verify(
            r => r.FirstOrDefaultAsync(
                It.Is<Expression<Func<Chat, bool>>>(expr => expr.Compile()(new Chat { ProjectId = query.ProjectId })),
                It.IsAny<CancellationToken>()),
            Times.Once());
        LoggerMockExtensions.VerifyLog(_loggerMock, LogLevel.Information, $"Getting chat by project ID '{query.ProjectId}'", Times.Once());
        LoggerMockExtensions.VerifyLog(_loggerMock, LogLevel.Information, $"Retrieved chat information by project ID {query.ProjectId}", Times.Once());
    }

    [Fact]
    public async Task Handle_WhenChatDoesNotExist_ReturnsNullAndLogs()
    {
        // Arrange
        var query = new GetChatByProjectIdQuery(Guid.NewGuid());

        _chatRepositoryMock
            .Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<Chat, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Chat?)null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeNull();
        _chatRepositoryMock.Verify(
            r => r.FirstOrDefaultAsync(
                It.Is<Expression<Func<Chat, bool>>>(expr => expr.Compile()(new Chat { ProjectId = query.ProjectId })),
                It.IsAny<CancellationToken>()),
            Times.Once());
        LoggerMockExtensions.VerifyLog(_loggerMock, LogLevel.Information, $"Getting chat by project ID '{query.ProjectId}'", Times.Once());
        LoggerMockExtensions.VerifyLog(_loggerMock, LogLevel.Warning, $"Chat with project ID '{query.ProjectId}' not found", Times.Once());
    }

    [Fact]
    public async Task Handle_WhenChatIsInactive_ReturnsNullAndLogs()
    {
        // Arrange
        var query = new GetChatByProjectIdQuery(Guid.NewGuid());
        var chat = new Chat { Id = Guid.NewGuid(), ProjectId = query.ProjectId, IsActive = false };

        _chatRepositoryMock
            .Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<Chat, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(chat);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeNull();
        _chatRepositoryMock.Verify(
            r => r.FirstOrDefaultAsync(
                It.Is<Expression<Func<Chat, bool>>>(expr => expr.Compile()(new Chat { ProjectId = query.ProjectId })),
                It.IsAny<CancellationToken>()),
            Times.Once());
        LoggerMockExtensions.VerifyLog(_loggerMock, LogLevel.Information, $"Getting chat by project ID '{query.ProjectId}'", Times.Once());
        LoggerMockExtensions.VerifyLog(_loggerMock, LogLevel.Warning, $"Chat with project ID '{query.ProjectId}' is inactive", Times.Once());
    }
}
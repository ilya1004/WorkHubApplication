using ChatService.Application.Exceptions;
using ChatService.Application.UseCases.ChatUseCases.Queries.GetChatById;
using ChatService.Domain.Abstractions.Repositories;
using ChatService.Tests.UnitTests.Extensions;

namespace ChatService.Tests.UnitTests.Tests.UseCases.ChatUseCases;

public class GetChatByIdQueryHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IChatsRepository> _chatRepositoryMock = new();
    private readonly Mock<ILogger<GetChatByIdQueryHandler>> _loggerMock = new();
    private readonly GetChatByIdQueryHandler _handler;

    public GetChatByIdQueryHandlerTests()
    {
        _unitOfWorkMock.Setup(u => u.ChatRepository).Returns(_chatRepositoryMock.Object);
        _handler = new GetChatByIdQueryHandler(_loggerMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_WhenChatExistsAndActive_ReturnsChatAndLogs()
    {
        // Arrange
        var query = new GetChatByIdQuery(Guid.NewGuid());
        var chat = new Chat { Id = query.Id, IsActive = true };

        _chatRepositoryMock
            .Setup(r => r.GetByIdAsync(query.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(chat);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().Be(chat);
        _chatRepositoryMock.Verify(r => r.GetByIdAsync(query.Id, It.IsAny<CancellationToken>()), Times.Once());
        LoggerMockExtensions.VerifyLog(_loggerMock, LogLevel.Information, $"Getting chat by ID '{query.Id}'", Times.Once());
        LoggerMockExtensions.VerifyLog(_loggerMock, LogLevel.Information, $"Retrieved chat information by ID {query.Id}", Times.Once());
    }

    [Fact]
    public async Task Handle_WhenChatDoesNotExist_ThrowsNotFoundException()
    {
        // Arrange
        var query = new GetChatByIdQuery(Guid.NewGuid());

        _chatRepositoryMock
            .Setup(r => r.GetByIdAsync(query.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Chat?)null);

        // Act
        Func<Task> act = async () => await _handler.Handle(query, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("Chat not found");
        _chatRepositoryMock.Verify(r => r.GetByIdAsync(query.Id, It.IsAny<CancellationToken>()), Times.Once());
        LoggerMockExtensions.VerifyLog(_loggerMock, LogLevel.Information, $"Getting chat by ID '{query.Id}'", Times.Once());
        LoggerMockExtensions.VerifyLog(_loggerMock, LogLevel.Warning, $"Chat with ID '{query.Id}' not found", Times.Once());
    }

    [Fact]
    public async Task Handle_WhenChatIsInactive_ThrowsNotFoundException()
    {
        // Arrange
        var query = new GetChatByIdQuery(Guid.NewGuid());
        var chat = new Chat { Id = query.Id, IsActive = false };

        _chatRepositoryMock
            .Setup(r => r.GetByIdAsync(query.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(chat);

        // Act
        Func<Task> act = async () => await _handler.Handle(query, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("Chat not found");
        _chatRepositoryMock.Verify(r => r.GetByIdAsync(query.Id, It.IsAny<CancellationToken>()), Times.Once());
        LoggerMockExtensions.VerifyLog(_loggerMock, LogLevel.Information, $"Getting chat by ID '{query.Id}'", Times.Once());
        LoggerMockExtensions.VerifyLog(_loggerMock, LogLevel.Warning, $"Chat with ID '{query.Id}' is inactive", Times.Once());
    }
}
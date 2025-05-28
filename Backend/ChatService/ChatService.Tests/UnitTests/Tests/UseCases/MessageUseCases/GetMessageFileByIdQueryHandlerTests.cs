using ChatService.Application.Exceptions;
using ChatService.Application.UseCases.MessageUseCases.Queries.GetMessageFileById;
using ChatService.Domain.Abstractions.BlobService;
using ChatService.Domain.Abstractions.Repositories;
using ChatService.Domain.Abstractions.UserContext;
using ChatService.Tests.UnitTests.Extensions;

namespace ChatService.Tests.UnitTests.Tests.UseCases.MessageUseCases;

public class GetMessageFileByIdQueryHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IChatsRepository> _chatRepositoryMock = new();
    private readonly Mock<IBlobService> _blobServiceMock = new();
    private readonly Mock<IUserContext> _userContextMock = new();
    private readonly Mock<ILogger<GetMessageFileByIdQueryHandler>> _loggerMock = new();
    private readonly GetMessageFileByIdQueryHandler _handler;

    public GetMessageFileByIdQueryHandlerTests()
    {
        _unitOfWorkMock.Setup(u => u.ChatRepository).Returns(_chatRepositoryMock.Object);
        _handler = new GetMessageFileByIdQueryHandler(
            _blobServiceMock.Object,
            _loggerMock.Object,
            _userContextMock.Object,
            _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_WhenChatExistsAndUserAuthorized_ReturnsFile()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var query = new GetMessageFileByIdQuery(ChatId: Guid.NewGuid(), FileId: Guid.NewGuid());
        var chat = new Chat { Id = query.ChatId, EmployerId = userId };
        var fileResponse = new FileResponse(new MemoryStream(new byte[] { 1, 2, 3 }), "application/pdf");

        _userContextMock.Setup(u => u.GetUserId()).Returns(userId);
        _chatRepositoryMock.Setup(r => r.GetByIdAsync(query.ChatId, It.IsAny<CancellationToken>())).ReturnsAsync(chat);
        _blobServiceMock.Setup(b => b.DownloadAsync(query.FileId, It.IsAny<CancellationToken>())).ReturnsAsync(fileResponse);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().Be(fileResponse);
        _chatRepositoryMock.Verify(r => r.GetByIdAsync(query.ChatId, It.IsAny<CancellationToken>()), Times.Once());
        _blobServiceMock.Verify(b => b.DownloadAsync(query.FileId, It.IsAny<CancellationToken>()), Times.Once());
        LoggerMockExtensions.VerifyLog(_loggerMock, LogLevel.Information, $"Getting file by ID: {query.FileId}", Times.Once());
        LoggerMockExtensions.VerifyLog(_loggerMock, LogLevel.Information, $"Successfully retrieved file with ID: {query.FileId}", Times.Once());
    }

    [Fact]
    public async Task Handle_WhenChatDoesNotExist_ThrowsNotFoundException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var query = new GetMessageFileByIdQuery(Guid.NewGuid(), Guid.NewGuid());

        _userContextMock.Setup(u => u.GetUserId()).Returns(userId);
        _chatRepositoryMock.Setup(r => r.GetByIdAsync(query.ChatId, It.IsAny<CancellationToken>())).ReturnsAsync((Chat?)null);

        // Act
        Func<Task> act = async () => await _handler.Handle(query, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("Chat not found");
        _chatRepositoryMock.Verify(r => r.GetByIdAsync(query.ChatId, It.IsAny<CancellationToken>()), Times.Once());
        _blobServiceMock.Verify(b => b.DownloadAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never());
        LoggerMockExtensions.VerifyLog(_loggerMock, LogLevel.Information, $"Getting file by ID: {query.FileId}", Times.Once());
        LoggerMockExtensions.VerifyLog(_loggerMock, LogLevel.Warning, $"Chat with ID '{query.ChatId}' not found", Times.Once());
    }

    [Fact]
    public async Task Handle_WhenUserNotAuthorized_ThrowsForbiddenException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var query = new GetMessageFileByIdQuery(Guid.NewGuid(), Guid.NewGuid());
        var chat = new Chat { Id = query.ChatId, EmployerId = Guid.NewGuid(), FreelancerId = Guid.NewGuid() };

        _userContextMock.Setup(u => u.GetUserId()).Returns(userId);
        _chatRepositoryMock.Setup(r => r.GetByIdAsync(query.ChatId, It.IsAny<CancellationToken>())).ReturnsAsync(chat);

        // Act
        Func<Task> act = async () => await _handler.Handle(query, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ForbiddenException>()
            .WithMessage("You do not have access to this chat");
        _chatRepositoryMock.Verify(r => r.GetByIdAsync(query.ChatId, It.IsAny<CancellationToken>()), Times.Once());
        _blobServiceMock.Verify(b => b.DownloadAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never());
        LoggerMockExtensions.VerifyLog(_loggerMock, LogLevel.Information, $"Getting file by ID: {query.FileId}", Times.Once());
        LoggerMockExtensions.VerifyLog(_loggerMock, LogLevel.Warning, $"You do not have access to chat with ID '{query.ChatId}'", Times.Once());
    }
}
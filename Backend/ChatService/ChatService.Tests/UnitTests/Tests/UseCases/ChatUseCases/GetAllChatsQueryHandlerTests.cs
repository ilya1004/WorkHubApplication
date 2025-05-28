using ChatService.Application.UseCases.ChatUseCases.Queries.GetAllChats;
using ChatService.Domain.Abstractions.Repositories;
using ChatService.Tests.UnitTests.Extensions;

namespace ChatService.Tests.UnitTests.Tests.UseCases.ChatUseCases;

public class GetAllChatsQueryHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IChatsRepository> _chatRepositoryMock = new();
    private readonly Mock<ILogger<GetAllChatsQueryHandler>> _loggerMock = new();
    private readonly GetAllChatsQueryHandler _handler;

    public GetAllChatsQueryHandlerTests()
    {
        _unitOfWorkMock.Setup(u => u.ChatRepository).Returns(_chatRepositoryMock.Object);
        _handler = new GetAllChatsQueryHandler(_unitOfWorkMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ReturnsPaginatedChatsAndLogs()
    {
        // Arrange
        var query = new GetAllChatsQuery(PageNo: 2, PageSize: 10);
        var chats = new List<Chat>
        {
            new Chat { Id = Guid.NewGuid() },
            new Chat { Id = Guid.NewGuid() }
        };
        var totalCount = 25;

        _chatRepositoryMock
            .Setup(r => r.PaginatedListAllAsync((query.PageNo - 1) * query.PageSize, query.PageSize, It.IsAny<CancellationToken>()))
            .ReturnsAsync(chats);
        _chatRepositoryMock
            .Setup(r => r.CountAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(totalCount);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().BeEquivalentTo(chats);
        result.PageNo.Should().Be(query.PageNo);
        result.PageSize.Should().Be(query.PageSize);
        result.TotalCount.Should().Be(totalCount);
        _chatRepositoryMock.Verify(r => r.PaginatedListAllAsync((query.PageNo - 1) * query.PageSize, query.PageSize, It.IsAny<CancellationToken>()), Times.Once());
        _chatRepositoryMock.Verify(r => r.CountAllAsync(It.IsAny<CancellationToken>()), Times.Once());
        LoggerMockExtensions.VerifyLog(_loggerMock, LogLevel.Information, $"Getting paginated chats list. Page {query.PageNo}, Size {query.PageSize}", Times.Once());
        LoggerMockExtensions.VerifyLog(_loggerMock, LogLevel.Information, $"Retrieved {chats.Count} chats out of {totalCount}", Times.Once());
    }
}
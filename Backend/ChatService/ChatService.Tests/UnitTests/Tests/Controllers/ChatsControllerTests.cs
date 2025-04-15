using ChatService.API.Contracts.CommonContracts;
using ChatService.API.Controllers;
using ChatService.Application.UseCases.ChatUseCases.Queries.GetAllChats;
using Microsoft.AspNetCore.Mvc;

namespace ChatService.Tests.UnitTests.Tests.Controllers;

public class ChatsControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly ChatsController _controller;

    public ChatsControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _controller = new ChatsController(_mediatorMock.Object);
    }

    [Fact]
    public async Task GetAllChats_ShouldReturnOk_WithPaginatedResult()
    {
        // Arrange
        var request = new GetPaginatedListRequest(1, 10);
        var expectedResult = new PaginatedResultModel<Chat>
        {
            Items = [new Chat { Id = Guid.NewGuid(), IsActive = true }],
            TotalCount = 1,
            PageNo = 1,
            PageSize = 10
        };
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetAllChatsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _controller.GetAllChats(request, CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeEquivalentTo(expectedResult);
        _mediatorMock.Verify(
            m => m.Send(It.Is<GetAllChatsQuery>(q => q.PageNo == 1 && q.PageSize == 10), It.IsAny<CancellationToken>()),
            Times.Once());
    }
}
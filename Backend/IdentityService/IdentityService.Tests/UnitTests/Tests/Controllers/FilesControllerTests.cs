using IdentityService.API.Controllers;
using IdentityService.BLL.UseCases.FileUseCases.Queries.GetImageById;
using IdentityService.BLL.UseCases.FileUseCases.Queries.GetImageByUserId;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.Tests.UnitTests.Tests.Controllers;

public class FilesControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly FilesController _controller;
    private readonly CancellationToken _cancellationToken;

    public FilesControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _controller = new FilesController(_mediatorMock.Object);
        _cancellationToken = CancellationToken.None;
    }

    [Fact]
    public async Task GetFileByUserId_ValidUserId_ReturnsFileResult()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var stream = new MemoryStream();
        var fileResponse = new FileResponseDto(stream, "image/jpeg");
        _mediatorMock.Setup(m => m.Send(It.Is<GetImageByUserIdQuery>(q => q.UserId == userId), _cancellationToken))
            .ReturnsAsync(fileResponse);

        // Act
        var result = await _controller.GetFileByUserId(userId, _cancellationToken);

        // Assert
        result.Should().BeOfType<FileStreamResult>()
            .Which.Should().Match<FileStreamResult>(r => r.FileStream == stream && r.ContentType == "image/jpeg");
        _mediatorMock.Verify(m => m.Send(It.Is<GetImageByUserIdQuery>(q => q.UserId == userId), _cancellationToken), Times.Once());
    }

    [Fact]
    public async Task GetFileById_ValidId_ReturnsFileResult()
    {
        // Arrange
        var id = Guid.NewGuid();
        var stream = new MemoryStream();
        var fileResponse = new FileResponseDto(stream, "image/png");
        _mediatorMock.Setup(m => m.Send(It.Is<GetImageByIdQuery>(q => q.Id == id), _cancellationToken))
            .ReturnsAsync(fileResponse);

        // Act
        var result = await _controller.GetFileById(id, _cancellationToken);

        // Assert
        result.Should().BeOfType<FileStreamResult>()
            .Which.Should().Match<FileStreamResult>(r => r.FileStream == stream && r.ContentType == "image/png");
        _mediatorMock.Verify(m => m.Send(It.Is<GetImageByIdQuery>(q => q.Id == id), _cancellationToken), Times.Once());
    }
}
using IdentityService.BLL.Abstractions.BlobService;
using IdentityService.BLL.UseCases.FileUseCases.Queries.GetImageById;
using IdentityService.Tests.UnitTests.Extensions;

namespace IdentityService.Tests.UnitTests.Tests.UseCases.FileUseCases;

public class GetImageByIdQueryHandlerTests
{
    private readonly Mock<IBlobService> _blobServiceMock;
    private readonly Mock<ILogger<GetImageByIdQueryHandler>> _loggerMock;
    private readonly GetImageByIdQueryHandler _handler;

    public GetImageByIdQueryHandlerTests()
    {
        _blobServiceMock = new Mock<IBlobService>();
        _loggerMock = new Mock<ILogger<GetImageByIdQueryHandler>>();

        _handler = new GetImageByIdQueryHandler(_blobServiceMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnFileResponse_WhenImageExists()
    {
        // Arrange
        var imageId = Guid.NewGuid();
        var command = new GetImageByIdQuery(imageId);
        var stream = new MemoryStream();
        var contentType = "image/jpeg";
        var fileResponse = new FileResponseDto(stream, contentType);

        _blobServiceMock.Setup(b => b.DownloadAsync(imageId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(fileResponse);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        var result = await act();
        result.Should().Be(fileResponse);
        _loggerMock.VerifyLog(LogLevel.Information, $"Successfully retrieved image with ID: {imageId}", Times.Once());
    }
}
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using IdentityService.BLL.Exceptions;
using IdentityService.BLL.Services.BlobService;
using IdentityService.BLL.Settings;
using IdentityService.Tests.UnitTests.Extensions;
using Microsoft.Extensions.Options;

namespace IdentityService.Tests.UnitTests.Tests.Services.BllServices;

public class BlobServiceTests
{
    private readonly Mock<BlobContainerClient> _containerClientMock;
    private readonly Mock<BlobClient> _blobClientMock;
    private readonly BlobService _service;

    public BlobServiceTests()
    {
        var blobServiceClientMock = new Mock<BlobServiceClient>();
        var loggerMock = new Mock<ILogger<BlobService>>();
        var optionsMock = new Mock<IOptions<AzuriteSettings>>();
        _containerClientMock = new Mock<BlobContainerClient>();
        _blobClientMock = new Mock<BlobClient>();

        optionsMock.Setup(o => o.Value).Returns(new AzuriteSettings
        {
            ImagesContainerName = "images",
            ConnectionString = null!
        });
        blobServiceClientMock.Setup(c => c.GetBlobContainerClient("images")).Returns(_containerClientMock.Object);

        _service = new BlobService(blobServiceClientMock.Object, optionsMock.Object, loggerMock.Object);
    }

    [Fact]
    public async Task UploadAsync_ShouldUploadFile_AndReturnFileId()
    {
        // Arrange
        var stream = new MemoryStream(new byte[] { 1, 2, 3 });
        var contentType = "image/jpeg";
        _containerClientMock.Setup(c => c.GetBlobClient(It.IsAny<string>())).Returns(_blobClientMock.Object);
        _blobClientMock.Setup(b => b.UploadAsync(
                stream, It.Is<BlobHttpHeaders>(h => h.ContentType == contentType), null, null, null, null, default, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Mock.Of<Response<BlobContentInfo>>());

        // Act
        var result = await _service.UploadAsync(stream, contentType);

        // Assert
        result.Should().NotBeEmpty();
        _blobClientMock.Verify(b => b.UploadAsync(
            stream, It.Is<BlobHttpHeaders>(h => h.ContentType == contentType), null, null, null, null, default, It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task UploadAsync_ShouldThrowNotFoundException_WhenContainerNotFound()
    {
        // Arrange
        var stream = new MemoryStream(new byte[] { 1, 2, 3 });
        var contentType = "image/jpeg";
        var fileId = Guid.NewGuid();
        _containerClientMock.Setup(c => c.GetBlobClient(fileId.ToString())).Returns(_blobClientMock.Object);
        _blobClientMock.Setup(b => b.UploadAsync(It.IsAny<Stream>(), It.IsAny<BlobHttpHeaders>(), null, null, null, null, default, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new RequestFailedException(404, "Container not found", "ContainerNotFound", null));
        
        // Act
        var act = () => _service.UploadAsync(stream, contentType);
        
        // Assert
        await act.Should().ThrowAsync<BadRequestException>();
    }

    [Fact]
    public async Task DownloadAsync_ShouldReturnFileResponse_WhenFileExists()
    {
        // Arrange
        var fileId = Guid.NewGuid();
        var contentType = "image/jpeg";
        var content = new byte[] { 1, 2, 3 };
        var downloadResult = BlobsModelFactory.BlobDownloadResult(BinaryData.FromBytes(content), new BlobDownloadDetails());
        _containerClientMock.Setup(c => c.GetBlobClient(fileId.ToString())).Returns(_blobClientMock.Object);
        _blobClientMock.Setup(b => b.DownloadContentAsync(It.IsAny<CancellationToken>())).ReturnsAsync(Response.FromValue(downloadResult, Mock.Of<Response>()));

        // Act
        var result = await _service.DownloadAsync(fileId);

        // Assert
        result.Should().NotBeNull();
        result.Stream.ToArray().Should().BeEquivalentTo(content);
    }

    [Fact]
    public async Task DownloadAsync_ShouldThrowNotFoundException_WhenFileNotFound()
    {
        // Arrange
        var fileId = Guid.NewGuid();
        _containerClientMock.Setup(c => c.GetBlobClient(fileId.ToString())).Returns(_blobClientMock.Object);
        _blobClientMock.Setup(b => b.DownloadContentAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new RequestFailedException(404, "Blob not found", "BlobNotFound", null));

        // Act
        var act = () => _service.DownloadAsync(fileId);
        
        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task DeleteAsync_ShouldDeleteFile_WhenFileExists()
    {
        // Arrange
        var fileId = Guid.NewGuid();
        _containerClientMock.Setup(c => c.GetBlobClient(fileId.ToString())).Returns(_blobClientMock.Object);
        _blobClientMock.Setup(b => b.DeleteIfExistsAsync(It.IsAny<DeleteSnapshotsOption>(), null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Response.FromValue(true, Mock.Of<Response>()));

        // Act
        await _service.DeleteAsync(fileId);

        // Assert
        _blobClientMock.Verify(b => b.DeleteIfExistsAsync(It.IsAny<DeleteSnapshotsOption>(), null, It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task DeleteAsync_ShouldLogWarning_WhenFileDoesNotExist()
    {
        // Arrange
        var fileId = Guid.NewGuid();
        _containerClientMock.Setup(c => c.GetBlobClient(fileId.ToString())).Returns(_blobClientMock.Object);
        _blobClientMock.Setup(b => b.DeleteIfExistsAsync(It.IsAny<DeleteSnapshotsOption>(), null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Response.FromValue(false, Mock.Of<Response>()));

        // Act
        await _service.DeleteAsync(fileId);

        // Assert
        _blobClientMock.Verify(b => b.DeleteIfExistsAsync(It.IsAny<DeleteSnapshotsOption>(), null, It.IsAny<CancellationToken>()), Times.Once());
    }
}
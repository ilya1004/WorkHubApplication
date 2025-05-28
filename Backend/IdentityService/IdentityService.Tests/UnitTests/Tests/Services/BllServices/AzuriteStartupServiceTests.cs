using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using IdentityService.BLL.Services.AzuriteStartupService;
using IdentityService.BLL.Settings;
using IdentityService.Tests.UnitTests.Extensions;
using Microsoft.Extensions.Options;

namespace IdentityService.Tests.UnitTests.Tests.Services.BllServices;

public class AzuriteStartupServiceTests
{
    private readonly Mock<BlobContainerClient> _containerClientMock;
    private readonly AzuriteStartupService _service;

    public AzuriteStartupServiceTests()
    {
        var blobServiceClientMock = new Mock<BlobServiceClient>();
        var loggerMock = new Mock<ILogger<AzuriteStartupService>>();
        var optionsMock = new Mock<IOptions<AzuriteSettings>>();
        _containerClientMock = new Mock<BlobContainerClient>();

        optionsMock.Setup(o => o.Value).Returns(new AzuriteSettings
        {
            ImagesContainerName = "images",
            ConnectionString = null!
        });
        blobServiceClientMock.Setup(c => c.GetBlobContainerClient("images")).Returns(_containerClientMock.Object);

        _service = new AzuriteStartupService(blobServiceClientMock.Object, loggerMock.Object, optionsMock.Object);
    }

    [Fact]
    public async Task CreateContainerIfNotExistAsync_ShouldCreateContainer_WhenItDoesNotExist()
    {
        // Arrange
        _containerClientMock.Setup(c => c.ExistsAsync(It.IsAny<CancellationToken>())).ReturnsAsync(Response.FromValue(false, null!));
        _containerClientMock.Setup(c => c.CreateIfNotExistsAsync(PublicAccessType.Blob, null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Mock.Of<Response<BlobContainerInfo>>());

        // Act
        await _service.CreateContainerIfNotExistAsync();

        // Assert
        _containerClientMock.Verify(c => c.CreateIfNotExistsAsync(PublicAccessType.Blob, null, null, It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task CreateContainerIfNotExistAsync_ShouldNotCreateContainer_WhenItExists()
    {
        // Arrange
        _containerClientMock.Setup(c => c.ExistsAsync(It.IsAny<CancellationToken>())).ReturnsAsync(Response.FromValue(true, null!));

        // Act
        await _service.CreateContainerIfNotExistAsync();

        // Assert
        _containerClientMock.Verify(c => c.CreateIfNotExistsAsync(It.IsAny<PublicAccessType>(), null, null, It.IsAny<CancellationToken>()), Times.Never());
    }
}
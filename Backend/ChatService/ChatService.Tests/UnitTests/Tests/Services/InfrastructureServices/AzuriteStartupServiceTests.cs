using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using ChatService.Infrastructure.Services.AzuriteStartupService;
using ChatService.Infrastructure.Settings;
using ChatService.Tests.UnitTests.Extensions;
using Microsoft.Extensions.Options;

namespace ChatService.Tests.UnitTests.Tests.Services.InfrastructureServices;

public class AzuriteStartupServiceTests
{
    private readonly Mock<BlobServiceClient> _blobServiceClientMock; 
    private readonly Mock<BlobContainerClient> _containerClientMock; 
    private readonly Mock<ILogger<AzuriteStartupService>> _loggerMock; 
    private readonly Mock<IOptions<AzuriteSettings>> _optionsMock; 
    private readonly AzuriteStartupService _service;
    
        public AzuriteStartupServiceTests()
    {
        _blobServiceClientMock = new Mock<BlobServiceClient>();
        _containerClientMock = new Mock<BlobContainerClient>();
        _loggerMock = new Mock<ILogger<AzuriteStartupService>>();
        _optionsMock = new Mock<IOptions<AzuriteSettings>>();

        _optionsMock.Setup(o => o.Value).Returns(new AzuriteSettings
        {
            ConnectionString = "UseDevelopmentStorage=true",
            FilesContainerName = "test-container"
        });

        _blobServiceClientMock
            .Setup(x => x.GetBlobContainerClient("test-container"))
            .Returns(_containerClientMock.Object);

        _service = new AzuriteStartupService(_blobServiceClientMock.Object, _loggerMock.Object, _optionsMock.Object);
    }

    [Fact]
    public async Task CreateContainerIfNotExistAsync_ShouldCreateContainer_WhenItDoesNotExist()
    {
        // Arrange
        _containerClientMock
            .Setup(x => x.ExistsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Response.FromValue(false, new Mock<Response>().Object));

        _containerClientMock
            .Setup(x => x.CreateIfNotExistsAsync(PublicAccessType.Blob, null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Response.FromValue(BlobsModelFactory.BlobContainerInfo(It.IsAny<ETag>(), It.IsAny<DateTimeOffset>()), new Mock<Response>().Object));

        // Act
        await _service.CreateContainerIfNotExistAsync();

        // Assert
        _containerClientMock.Verify(x => x.ExistsAsync(It.IsAny<CancellationToken>()), Times.Once());
        _containerClientMock.Verify(x => x.CreateIfNotExistsAsync(PublicAccessType.Blob, null, null, It.IsAny<CancellationToken>()), Times.Once());

        _loggerMock.VerifyLog(LogLevel.Information, "Checking container 'test-container' existence", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, "Creating container 'test-container'", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, "Container 'test-container' created successfully", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, "Container 'test-container' already exists", Times.Never());
    }

    [Fact]
    public async Task CreateContainerIfNotExistAsync_ShouldNotCreateContainer_WhenItExists()
    {
        // Arrange
        _containerClientMock
            .Setup(x => x.ExistsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Response.FromValue(true, new Mock<Response>().Object));

        // Act
        await _service.CreateContainerIfNotExistAsync();

        // Assert
        _containerClientMock.Verify(x => x.ExistsAsync(It.IsAny<CancellationToken>()), Times.Once());
        _containerClientMock.Verify(x => x.CreateIfNotExistsAsync(It.IsAny<PublicAccessType>(), null, null, It.IsAny<CancellationToken>()), Times.Never());

        _loggerMock.VerifyLog(LogLevel.Information, "Checking container 'test-container' existence", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, "Container 'test-container' already exists", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, "Creating container 'test-container'", Times.Never());
        _loggerMock.VerifyLog(LogLevel.Information, "Container 'test-container' created successfully", Times.Never());
    }

    [Fact]
    public async Task CreateContainerIfNotExistAsync_ShouldThrowException_WhenCreationFails()
    {
        // Arrange
        _containerClientMock
            .Setup(x => x.ExistsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Response.FromValue(false, new Mock<Response>().Object));

        _containerClientMock
            .Setup(x => x.CreateIfNotExistsAsync(PublicAccessType.Blob, null, null, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Blob storage error"));

        // Act
        Func<Task> act = async () => await _service.CreateContainerIfNotExistAsync();

        // Assert
        await act.Should().ThrowAsync<Exception>().WithMessage("Blob storage error");

        _loggerMock.VerifyLog(LogLevel.Information, "Checking container 'test-container' existence", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, "Creating container 'test-container'", Times.Once());
        _loggerMock.VerifyLog(LogLevel.Information, "Container 'test-container' created successfully", Times.Never());
        _loggerMock.VerifyLog(LogLevel.Information, "Container 'test-container' already exists", Times.Never());
    }
}

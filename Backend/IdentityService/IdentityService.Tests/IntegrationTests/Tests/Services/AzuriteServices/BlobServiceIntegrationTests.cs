using System.Text;
using Azure.Storage.Blobs;
using IdentityService.BLL.Abstractions.BlobService;
using IdentityService.BLL.Exceptions;
using IdentityService.BLL.Services.BlobService;
using IdentityService.BLL.Settings;
using IdentityService.Tests.IntegrationTests.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace IdentityService.Tests.IntegrationTests.Tests.Services.AzuriteServices;

public class BlobServiceIntegrationTests(
    IntegrationTestsFixture fixture) : IClassFixture<IntegrationTestsFixture>
{
    [Fact]
    public async Task UploadAsync_ShouldUploadFileAndReturnFileId()
    {
        // Arrange
        using var scope = fixture.Factory.Services.CreateScope();
        var blobService = scope.ServiceProvider.GetRequiredService<IBlobService>();
        var content = "Test content";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        var contentType = "text/plain";

        // Act
        var fileId = await blobService.UploadAsync(stream, contentType);

        // Assert
        fileId.Should().NotBeEmpty();

        var blobServiceClient = scope.ServiceProvider.GetRequiredService<BlobServiceClient>();
        var containerClient = blobServiceClient.GetBlobContainerClient("user-images");
        var blobClient = containerClient.GetBlobClient(fileId.ToString());
        var exists = await blobClient.ExistsAsync();
        exists.Value.Should().BeTrue();
    }

    [Fact]
    public async Task UploadAsync_WithImageContentType_ShouldUploadSuccessfully()
    {
        // Arrange
        using var scope = fixture.Factory.Services.CreateScope();
        var blobService = scope.ServiceProvider.GetRequiredService<IBlobService>();
        var imageContent = new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 };
        using var stream = new MemoryStream(imageContent);
        var contentType = "image/jpeg";

        // Act
        var fileId = await blobService.UploadAsync(stream, contentType);

        // Assert
        fileId.Should().NotBeEmpty();

        var blobServiceClient = scope.ServiceProvider.GetRequiredService<BlobServiceClient>();
        var containerClient = blobServiceClient.GetBlobContainerClient("user-images");
        var blobClient = containerClient.GetBlobClient(fileId.ToString());
        var properties = await blobClient.GetPropertiesAsync();
        properties.Value.ContentType.Should().Be("image/jpeg");
    }

    [Fact]
    public async Task UploadAsync_WhenContainerDoesNotExist_ShouldThrowNotFoundException()
    {
        // Arrange
        using var scope = fixture.Factory.Services.CreateScope();
        var blobServiceClient = scope.ServiceProvider.GetRequiredService<BlobServiceClient>();
        var options = Options.Create(new AzuriteSettings
        {
            ImagesContainerName = "nonexistent-container",
            ConnectionString = null
        });
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<BlobService>>();
        var blobService = new BlobService(blobServiceClient, options, logger);
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes("Test"));
        var contentType = "text/plain";

        // Act
        var act = async () => await blobService.UploadAsync(stream, contentType);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("The specified blob container does not exist.");
    }

    [Fact]
    public async Task DownloadAsync_ShouldDownloadFileWithCorrectContent()
    {
        // Arrange
        using var scope = fixture.Factory.Services.CreateScope();
        var blobService = scope.ServiceProvider.GetRequiredService<IBlobService>();
        var content = "Downloadable content";
        using var uploadStream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        var contentType = "text/plain";
        var fileId = await blobService.UploadAsync(uploadStream, contentType);

        // Act
        var fileResponse = await blobService.DownloadAsync(fileId);

        // Assert
        fileResponse.Should().NotBeNull();
        fileResponse.ContentType.Should().Be("text/plain");
        using var reader = new StreamReader(fileResponse.Stream);
        var downloadedContent = await reader.ReadToEndAsync();
        downloadedContent.Should().Be(content);
    }

    [Fact]
    public async Task DownloadAsync_WhenFileDoesNotExist_ShouldThrowNotFoundException()
    {
        // Arrange
        using var scope = fixture.Factory.Services.CreateScope();
        var blobService = scope.ServiceProvider.GetRequiredService<IBlobService>();
        var nonExistentFileId = Guid.NewGuid();

        // Act
        var act = async () => await blobService.DownloadAsync(nonExistentFileId);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"The file with ID {nonExistentFileId} not found in the blob storage.");
    }

    [Fact]
    public async Task DeleteAsync_ShouldDeleteExistingFile()
    {
        // Arrange
        using var scope = fixture.Factory.Services.CreateScope();
        var blobService = scope.ServiceProvider.GetRequiredService<IBlobService>();
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes("To be deleted"));
        var contentType = "text/plain";
        var fileId = await blobService.UploadAsync(stream, contentType);

        // Act
        await blobService.DeleteAsync(fileId);

        // Assert
        var blobServiceClient = scope.ServiceProvider.GetRequiredService<BlobServiceClient>();
        var containerClient = blobServiceClient.GetBlobContainerClient("user-images");
        var blobClient = containerClient.GetBlobClient(fileId.ToString());
        var exists = await blobClient.ExistsAsync();
        exists.Value.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteAsync_WhenFileDoesNotExist_ShouldNotThrow()
    {
        // Arrange
        using var scope = fixture.Factory.Services.CreateScope();
        var blobService = scope.ServiceProvider.GetRequiredService<IBlobService>();
        var nonExistentFileId = Guid.NewGuid();

        // Act
        var act = async () => await blobService.DeleteAsync(nonExistentFileId);

        // Assert
        await act.Should().NotThrowAsync();
    }
}
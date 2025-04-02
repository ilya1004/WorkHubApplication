using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using IdentityService.BLL.Abstractions.AzuriteStartupService;
using IdentityService.BLL.Settings;

namespace IdentityService.BLL.Services.AzuriteStartupService;

public class AzuriteStartupService(
    BlobServiceClient blobServiceClient,
    ILogger<AzuriteStartupService> logger,
    IOptions<AzuriteSettings> options) : IAzuriteStartupService
{
    public async Task CreateContainerIfNotExistAsync()
    {
        var containerName = options.Value.ImagesContainerName;
        
        logger.LogInformation("Checking container '{ContainerName}' existence", containerName);
        
        var containerClient = blobServiceClient.GetBlobContainerClient(containerName);

        if (!await containerClient.ExistsAsync())
        {
            logger.LogInformation("Creating container '{ContainerName}'", containerName);
            
            await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob, metadata: null);
            
            logger.LogInformation("Container '{ContainerName}' created successfully", containerName);
        }
        else
        {
            logger.LogInformation("Container '{ContainerName}' already exists", containerName);
        }
    }
}
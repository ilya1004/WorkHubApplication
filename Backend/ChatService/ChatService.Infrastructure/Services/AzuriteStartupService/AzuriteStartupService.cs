using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using ChatService.Domain.Abstractions.AzuriteStartupService;
using ChatService.Infrastructure.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ChatService.Infrastructure.Services.AzuriteStartupService;

public class AzuriteStartupService(
    BlobServiceClient blobServiceClient,
    ILogger<AzuriteStartupService> logger,
    IOptions<AzuriteSettings> options) : IAzuriteStartupService
{
    public async Task CreateContainerIfNotExistAsync()
    {
        var containerName = options.Value.FilesContainerName;
        
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
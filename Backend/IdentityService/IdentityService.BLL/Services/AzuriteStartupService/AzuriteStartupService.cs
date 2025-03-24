using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using IdentityService.BLL.Abstractions.AzuriteStartupService;
using Microsoft.Extensions.Configuration;

namespace IdentityService.BLL.Services.AzuriteStartupService;

public class AzuriteStartupService(IConfiguration configuration, BlobServiceClient blobServiceClient) : IAzuriteStartupService
{
    public async Task CreateContainerIfNotExistAsync()
    {
        var containerName = configuration["Azurite:ImagesContainerName"];
        var containerClient = blobServiceClient.GetBlobContainerClient(containerName);

        await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob, metadata: null);
    }
}
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using ChatService.Application.Exceptions;
using ChatService.Domain.Abstractions.BlobService;
using ChatService.Infrastructure.Settings;
using Microsoft.Extensions.Options;

namespace ChatService.Infrastructure.Services.BlobService;

public class BlobService(
    BlobServiceClient blobServiceClient, 
    IOptions<AzuriteSettings> options) : IBlobService
{
    private readonly string _containerName = options.Value.FilesContainerName;

    public async Task<Guid> UploadAsync(Stream stream, string contentType, CancellationToken cancellationToken = default)
    {
        try
        {
            var blobContainerClient = blobServiceClient.GetBlobContainerClient(_containerName);

            var fileId = Guid.NewGuid();
            var blobClient = blobContainerClient.GetBlobClient(fileId.ToString());

            await blobClient.UploadAsync(stream, new BlobHttpHeaders { ContentType = contentType }, cancellationToken: cancellationToken);

            return fileId;
        }
        catch (RequestFailedException ex) when (ex.ErrorCode == BlobErrorCode.ContainerNotFound)
        {
            throw new NotFoundException("The specified blob container does not exist.");
        }
        catch (Exception ex)
        {
            throw new BadRequestException($"An error occurred during the upload process: {ex.Message}");
        }
    }

    public async Task<FileResponse> DownloadAsync(Guid fileId, CancellationToken cancellationToken = default)
    {
        try
        {
            var blobContainerClient = blobServiceClient.GetBlobContainerClient(_containerName);

            var blobClient = blobContainerClient.GetBlobClient(fileId.ToString());

            Response<BlobDownloadResult> fileResponse = await blobClient.DownloadContentAsync(cancellationToken);

            return new FileResponse(fileResponse.Value.Content.ToStream(), fileResponse.Value.Details.ContentType);
        }
        catch (RequestFailedException ex) when (ex.ErrorCode == BlobErrorCode.BlobNotFound)
        {
            throw new NotFoundException($"The file with ID {fileId} not found in the blob storage.");
        }
        catch (Exception ex)
        {
            throw new BadRequestException($"An error occurred during the download process: {ex.Message}");
        }
    }

    public async Task DeleteAsync(Guid fileId, CancellationToken cancellationToken = default)
    {
        try
        {
            var blobContainerClient = blobServiceClient.GetBlobContainerClient(_containerName);

            var blobClient = blobContainerClient.GetBlobClient(fileId.ToString());

            await blobClient.DeleteIfExistsAsync(cancellationToken: cancellationToken);
        }
        catch (RequestFailedException ex) when (ex.ErrorCode == BlobErrorCode.BlobNotFound)
        {
            throw new NotFoundException($"The file with ID {fileId} not found in the blob storage.");
        }
        catch (Exception ex)
        {
            throw new BadRequestException($"An error occurred during the deletion process: {ex.Message}");
        }
    }
}
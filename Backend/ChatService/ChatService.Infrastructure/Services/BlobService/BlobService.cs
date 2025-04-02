using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using ChatService.Application.Exceptions;
using ChatService.Domain.Abstractions.BlobService;
using ChatService.Infrastructure.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ChatService.Infrastructure.Services.BlobService;

public class BlobService(
    BlobServiceClient blobServiceClient,
    IOptions<AzuriteSettings> options,
    ILogger<BlobService> logger) : IBlobService
{
    private readonly string _containerName = options.Value.FilesContainerName;

    public async Task<Guid> UploadAsync(Stream stream, string contentType, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Starting file upload. Content type: {ContentType}, Stream length: {Length}",
            contentType, stream.Length);

        try
        {
            var blobContainerClient = blobServiceClient.GetBlobContainerClient(_containerName);
            var fileId = Guid.NewGuid();
            var blobClient = blobContainerClient.GetBlobClient(fileId.ToString());

            await blobClient.UploadAsync(
                stream,
                new BlobHttpHeaders { ContentType = contentType },
                cancellationToken: cancellationToken);

            logger.LogInformation("File {FileId} uploaded successfully. Size: {Size} bytes", fileId, stream.Length);

            return fileId;
        }
        catch (RequestFailedException ex) when (ex.ErrorCode == BlobErrorCode.ContainerNotFound)
        {
            logger.LogError("Container {Container} not found", _containerName);
            
            throw new NotFoundException("The specified blob container does not exist.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "File upload failed");
            
            throw new BadRequestException($"An error occurred during the upload process: {ex.Message}");
        }
    }

    public async Task<FileResponse> DownloadAsync(Guid fileId, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Starting download for file {FileId}", fileId);

        try
        {
            var blobContainerClient = blobServiceClient.GetBlobContainerClient(_containerName);
            var blobClient = blobContainerClient.GetBlobClient(fileId.ToString());

            Response<BlobDownloadResult> fileResponse = await blobClient.DownloadContentAsync(cancellationToken);

            logger.LogInformation("File {FileId} downloaded successfully. Content type: {ContentType}", 
                fileId, fileResponse.Value.Details.ContentType);

            return new FileResponse(
                fileResponse.Value.Content.ToStream(),
                fileResponse.Value.Details.ContentType);
        }
        catch (RequestFailedException ex) when (ex.ErrorCode == BlobErrorCode.BlobNotFound)
        {
            logger.LogWarning("File {FileId} not found in container {Container}", fileId, _containerName);
            
            throw new NotFoundException($"The file with ID {fileId} not found in the blob storage.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "File download failed for {FileId}", fileId);
            
            throw new BadRequestException($"An error occurred during the download process: {ex.Message}");
        }
    }

    public async Task DeleteAsync(Guid fileId, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Starting deletion of file {FileId}", fileId);

        try
        {
            var blobContainerClient = blobServiceClient.GetBlobContainerClient(_containerName);
            var blobClient = blobContainerClient.GetBlobClient(fileId.ToString());

            var result = await blobClient.DeleteIfExistsAsync(cancellationToken: cancellationToken);

            if (result.Value)
            {
                logger.LogInformation("File {FileId} deleted successfully", fileId);
            }
            else
            {
                logger.LogWarning("File {FileId} did not exist when attempting to delete", fileId);
            }
        }
        catch (RequestFailedException ex) when (ex.ErrorCode == BlobErrorCode.BlobNotFound)
        {
            logger.LogWarning("File {FileId} not found during deletion attempt", fileId);
            
            throw new NotFoundException($"The file with ID {fileId} not found in the blob storage.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "File deletion failed for {FileId}", fileId);
            
            throw new BadRequestException($"An error occurred during the deletion process: {ex.Message}");
        }
    }
}
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using IdentityService.BLL.DTOs;
using Microsoft.Extensions.Configuration;

namespace IdentityService.BLL.Services.BlobService;

public class BlobService : IBlobService
{
    private readonly string _containerName;
    private readonly BlobServiceClient _blobServiceClient;

    public BlobService(BlobServiceClient blobServiceClient, IConfiguration configuration)
    {
        _blobServiceClient = blobServiceClient;
        _containerName = configuration["Azurite:ImagesContainerName"]!;
    }

    public async Task<Guid> UploadAsync(Stream stream, string contentType, CancellationToken cancellationToken = default)
    {
        try
        {
            var blobContainerClient = _blobServiceClient.GetBlobContainerClient(_containerName);

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

    public async Task<FileResponseDto> DownloadAsync(Guid fileId, CancellationToken cancellationToken = default)
    {
        try
        {
            var blobContainerClient = _blobServiceClient.GetBlobContainerClient(_containerName);

            var blobClient = blobContainerClient.GetBlobClient(fileId.ToString());

            Response<BlobDownloadResult> fileResponse = await blobClient.DownloadContentAsync(cancellationToken);

            return new FileResponseDto(fileResponse.Value.Content.ToStream(), fileResponse.Value.Details.ContentType);
        }
        catch (RequestFailedException ex) when (ex.ErrorCode == BlobErrorCode.BlobNotFound)
        {
            throw new NotFoundException($"The file with ID {fileId} was not found in the blob storage.");
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
            var blobContainerClient = _blobServiceClient.GetBlobContainerClient(_containerName);

            var blobClient = blobContainerClient.GetBlobClient(fileId.ToString());

            await blobClient.DeleteIfExistsAsync(cancellationToken: cancellationToken);
        }
        catch (RequestFailedException ex) when (ex.ErrorCode == BlobErrorCode.BlobNotFound)
        {
            throw new NotFoundException($"The file with ID {fileId} was not found in the blob storage.");
        }
        catch (Exception ex)
        {
            throw new BadRequestException($"An error occurred during the deletion process: {ex.Message}");
        }
    }
}
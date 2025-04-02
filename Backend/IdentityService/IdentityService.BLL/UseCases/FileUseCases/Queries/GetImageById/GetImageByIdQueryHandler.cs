using IdentityService.BLL.Abstractions.BlobService;
using IdentityService.BLL.DTOs;

namespace IdentityService.BLL.UseCases.FileUseCases.Queries.GetImageById;

public class GetImageByIdQueryHandler(
    IBlobService blobService,
    ILogger<GetImageByIdQueryHandler> logger) : IRequestHandler<GetImageByIdQuery, FileResponseDto>
{
    public async Task<FileResponseDto> Handle(GetImageByIdQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting image by ID: {ImageId}", request.Id);
        
        var result = await blobService.DownloadAsync(request.Id, cancellationToken);
        
        logger.LogInformation("Successfully retrieved image with ID: {ImageId}", request.Id);
        
        return result;
    }
}
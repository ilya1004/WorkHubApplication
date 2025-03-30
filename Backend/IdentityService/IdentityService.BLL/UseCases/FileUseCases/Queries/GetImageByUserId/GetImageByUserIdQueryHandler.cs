using IdentityService.BLL.Abstractions.BlobService;
using IdentityService.BLL.DTOs;

namespace IdentityService.BLL.UseCases.FileUseCases.Queries.GetImageByUserId;

public class GetImageByUserIdQueryHandler(
    UserManager<AppUser> userManager,
    IBlobService blobService,
    ILogger<GetImageByUserIdQueryHandler> logger) : IRequestHandler<GetImageByUserIdQuery, FileResponseDto>
{
    public async Task<FileResponseDto> Handle(GetImageByUserIdQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting image for user ID: {UserId}", request.UserId);

        var user = await userManager.FindByIdAsync(request.UserId.ToString());

        if (user is null)
        {
            logger.LogWarning("User with ID {UserId} not found", request.UserId);
            
            throw new NotFoundException($"User with ID '{request.UserId}' not found");
        }

        if (user.ImageUrl is null)
        {
            logger.LogWarning("User with ID {UserId} doesn't have an image", request.UserId);
            
            throw new NotFoundException($"User with ID {request.UserId} don't have an image");
        }

        logger.LogInformation("Downloading image for user ID: {UserId}", request.UserId);
        
        var result = await blobService.DownloadAsync(Guid.Parse(user.ImageUrl), cancellationToken);
        
        logger.LogInformation("Successfully retrieved image for user ID: {UserId}", request.UserId);
        
        return result;
    }
}
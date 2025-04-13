using IdentityService.BLL.Abstractions.BlobService;
using IdentityService.BLL.Abstractions.UserContext;

namespace IdentityService.BLL.UseCases.UserUseCases.Commands.UpdateEmployerProfile;

public class UpdateEmployerProfileCommandHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IBlobService blobService,
    IUserContext userContext,
    ILogger<UpdateEmployerProfileCommandHandler> logger) : IRequestHandler<UpdateEmployerProfileCommand>
{
    public async Task Handle(UpdateEmployerProfileCommand request, CancellationToken cancellationToken)
    {
        var userId = userContext.GetUserId();
        
        logger.LogInformation("Updating employer profile for user ID: {UserId}", userId);

        var user = await unitOfWork.UsersRepository.GetByIdAsync(
            userId,
            true,
            cancellationToken,
            u => u.EmployerProfile!,
            u => u.EmployerProfile!.Industry!);  

        if (user is null)
        {
            logger.LogWarning("User with ID {UserId} not found", userId);
        
            throw new NotFoundException($"User with ID '{userId}' not found");
        }
        
        mapper.Map(request.EmployerProfile, user.EmployerProfile);

        if (request.EmployerProfile.IndustryId.HasValue)
        {
            if (request.EmployerProfile.IndustryId.Value != user.EmployerProfile!.Industry?.Id)
            {
                var industry = await unitOfWork.EmployerIndustriesRepository.GetByIdAsync(
                    request.EmployerProfile.IndustryId.Value, cancellationToken);

                if (industry is null)
                {
                    logger.LogWarning("Industry with ID {IndustryId} not found", request.EmployerProfile.IndustryId);
                    
                    throw new NotFoundException($"Industry with ID '{request.EmployerProfile.IndustryId}' not found");
                }

                user.EmployerProfile!.Industry = industry;
            }
        }
        else if (user.EmployerProfile!.Industry is not null)
        {
            user.EmployerProfile!.Industry = null;
        }
        
        if (request.EmployerProfile.ResetImage)
        {
            user.ImageUrl = null;
        }

        if (request.FileStream is not null && request.ContentType is not null)
        {
            if (!request.ContentType.StartsWith("image/"))
            {
                logger.LogWarning("Invalid file type: {ContentType}", request.ContentType);
            
                throw new BadRequestException("Only image files are allowed.");
            }
            
            if (!string.IsNullOrEmpty(user.ImageUrl) && Guid.TryParse(user.ImageUrl, out var imageId))
            {
                logger.LogInformation("Deleting old image with ID: {ImageId}", imageId);
                
                await blobService.DeleteAsync(imageId, cancellationToken);
            }

            logger.LogInformation("Uploading new image");
            
            var imageFileId = await blobService.UploadAsync(
                request.FileStream,
                request.ContentType!,
                cancellationToken);

            user.ImageUrl = imageFileId.ToString();
        }

        await unitOfWork.SaveAllAsync(cancellationToken);

        logger.LogInformation("Successfully updated employer profile for user ID: {UserId}", userId);
    }
}
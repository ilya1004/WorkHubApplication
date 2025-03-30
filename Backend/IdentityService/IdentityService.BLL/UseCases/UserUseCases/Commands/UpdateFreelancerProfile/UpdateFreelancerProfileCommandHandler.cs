using IdentityService.BLL.Abstractions.BlobService;
using IdentityService.BLL.Abstractions.UserContext;
using IdentityService.BLL.Services.BlobService;
using IdentityService.DAL.Abstractions.Repositories;

namespace IdentityService.BLL.UseCases.UserUseCases.Commands.UpdateFreelancerProfile;

public class UpdateFreelancerProfileCommandHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IBlobService blobService,
    IUserContext userContext,
    ILogger<UpdateFreelancerProfileCommandHandler> logger) : IRequestHandler<UpdateFreelancerProfileCommand>
{
    public async Task Handle(UpdateFreelancerProfileCommand request, CancellationToken cancellationToken)
    {
        var userId = userContext.GetUserId();
        
        logger.LogInformation("Updating freelancer profile for user ID: {UserId}", userId);

        var user = await unitOfWork.UsersRepository.GetByIdAsync(
            userId,
            cancellationToken,
            u => u.FreelancerProfile!,
            u => u.FreelancerProfile!.Skills);

        if (user is null)
        {
            logger.LogWarning("User with ID {UserId} not found", userId);
            throw new NotFoundException($"User with ID '{userId}' not found");
        }

        logger.LogInformation("Mapping freelancer profile changes");
        
        mapper.Map(request.FreelancerProfile, user.FreelancerProfile);

        logger.LogInformation("Updating freelancer skills");
        
        var skills = await unitOfWork.FreelancerSkillsRepository.ListAsync(
            s => request.FreelancerProfile.SkillIds.Contains(s.Id), cancellationToken);

        user.FreelancerProfile!.Skills = skills.ToList();

        if (request.FreelancerProfile.ResetImage)
        {
            logger.LogInformation("Resetting user image");
        
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

        await unitOfWork.UsersRepository.UpdateAsync(user, cancellationToken);
        await unitOfWork.SaveAllAsync(cancellationToken);

        logger.LogInformation("Successfully updated freelancer profile for user ID: {UserId}", userId);
    }
}
using IdentityService.BLL.Abstractions.BlobService;
using IdentityService.BLL.Abstractions.UserContext;

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
            true,
            cancellationToken,
            u => u.FreelancerProfile!,
            u => u.FreelancerProfile!.Skills);  

        if (user is null)
        {
            throw new NotFoundException($"User with ID '{userId}' not found");
        }
        
        mapper.Map(request.FreelancerProfile, user.FreelancerProfile);

        if (request.FreelancerProfile.SkillIds is not null)
        {
            var newSkillIds = request.FreelancerProfile.SkillIds.ToList();
            var currentSkills = user.FreelancerProfile!.Skills.ToList();

            var skillsToRemove = currentSkills.Where(s => !newSkillIds.Contains(s.Id)).ToList();
            foreach (var skill in skillsToRemove)
            {
                user.FreelancerProfile.Skills.Remove(skill);
            }

            var allPotentialSkills = await unitOfWork.FreelancerSkillsRepository.ListAsync(
                s => newSkillIds.Contains(s.Id), cancellationToken);

            var skillsToAdd = allPotentialSkills.Where(s =>
                !currentSkills.Select(cs => cs.Id).Contains(s.Id)).ToList();

            foreach (var skill in skillsToAdd)
            {
                user.FreelancerProfile.Skills.Add(skill);
            }
        }
        else
        {
            var skillsToRemove = user.FreelancerProfile!.Skills.ToList();
            
            foreach (var skill in skillsToRemove)
            {
                user.FreelancerProfile.Skills.Remove(skill);
            }
        }

        if (request.FreelancerProfile.ResetImage)
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
                request.ContentType,
                cancellationToken);

            user.ImageUrl = imageFileId.ToString();
        }
        
        await unitOfWork.SaveAllAsync(cancellationToken);

        logger.LogInformation("Successfully updated freelancer profile for user ID: {UserId}", userId);
    }
}
using IdentityService.BLL.Services.BlobService;
using IdentityService.DAL.Abstractions.Repositories;

namespace IdentityService.BLL.UseCases.UserUseCases.Commands.UpdateFreelancerProfile;

public class UpdateFreelancerProfileCommandHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IBlobService blobService) : IRequestHandler<UpdateFreelancerProfileCommand>
{
    public async Task Handle(UpdateFreelancerProfileCommand request, CancellationToken cancellationToken)
    {
        var user = await unitOfWork.UsersRepository.GetByIdAsync(
            request.Id, cancellationToken,
            u => u.FreelancerProfile!, u => u.FreelancerProfile!.Skills);

        if (user is null)
        {
            throw new NotFoundException($"User with ID '{request.Id}' not found");
        }

        mapper.Map(request.FreelancerProfile, user.FreelancerProfile);

        var skills = await unitOfWork.FreelancerSkillsRepository.ListAsync(
            s => request.FreelancerProfile.SkillIds.Contains(s.Id), cancellationToken);

        user.FreelancerProfile!.Skills = skills.ToList();

        if (request.FreelancerProfile.ResetImage)
        {
            user.ImageUrl = null;
        }

        if (request.FileStream is not null)
        {
            if (!string.IsNullOrEmpty(user.ImageUrl) && Guid.TryParse(user.ImageUrl, out Guid imageId))
            {
                await blobService.DeleteAsync(imageId, cancellationToken);
            }

            var imageFileId = await blobService.UploadAsync(
                request.FileStream,
                request.ContentType!,
                cancellationToken);

            user.ImageUrl = imageFileId.ToString();
        }

        await unitOfWork.UsersRepository.UpdateAsync(user, cancellationToken);
        await unitOfWork.SaveAllAsync(cancellationToken);
    }
}

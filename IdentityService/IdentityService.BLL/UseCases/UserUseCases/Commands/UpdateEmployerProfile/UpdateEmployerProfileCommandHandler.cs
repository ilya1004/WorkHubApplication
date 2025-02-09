using IdentityService.BLL.Services.BlobService;

namespace IdentityService.BLL.UseCases.UserUseCases.Commands.UpdateEmployerProfile;

public class UpdateEmployerProfileCommandHandler(
    UserManager<AppUser> userManager, 
    IUnitOfWork unitOfWork, 
    IMapper mapper,
    IBlobService blobService) : IRequestHandler<UpdateEmployerProfileCommand>
{
    public async Task Handle(UpdateEmployerProfileCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Id.ToString());

        if (user == null)
        {
            throw new NotFoundException($"User with ID '{request.Id}' not found");
        }

        mapper.Map(request.EmployerProfile, user.EmployerProfile);

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
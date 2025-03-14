using IdentityService.BLL.Services.BlobService;
using IdentityService.DAL.Abstractions.Repositories;
using IdentityService.DAL.Abstractions.UserContext;

namespace IdentityService.BLL.UseCases.UserUseCases.Commands.UpdateEmployerProfile;

public class UpdateEmployerProfileCommandHandler(
    UserManager<AppUser> userManager,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IBlobService blobService,
    IUserContext userContext) : IRequestHandler<UpdateEmployerProfileCommand>
{
    public async Task Handle(UpdateEmployerProfileCommand request, CancellationToken cancellationToken)
    {
        var userId = userContext.GetUserId();

        var user = await userManager.FindByIdAsync(userId.ToString());

        if (user is null) throw new NotFoundException($"User with ID '{userId}' not found");

        mapper.Map(request.EmployerProfile, user.EmployerProfile);

        if (request.EmployerProfile.ResetImage) user.ImageUrl = null;

        if (request.FileStream is not null)
        {
            if (!string.IsNullOrEmpty(user.ImageUrl) && Guid.TryParse(user.ImageUrl, out var imageId))
                await blobService.DeleteAsync(imageId, cancellationToken);

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
using IdentityService.BLL.DTOs;
using IdentityService.BLL.Services.BlobService;

namespace IdentityService.BLL.UseCases.FileUseCases.Queries.GetImageByUserId;

public class GetImageByUserIdQueryHandler(
    UserManager<AppUser> userManager,
    IBlobService blobService) : IRequestHandler<GetImageByUserIdQuery, FileResponseDto>
{
    public async Task<FileResponseDto> Handle(GetImageByUserIdQuery request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId.ToString());

        if (user is null)
        {
            throw new NotFoundException($"User with ID '{request.UserId}' not found");
        }

        if (user.ImageUrl is null)
        {
            throw new NotFoundException($"User with ID {request.UserId} don't have an image");
        }

        var result = await blobService.DownloadAsync(Guid.Parse(user.ImageUrl), cancellationToken);

        return result;
    }
}

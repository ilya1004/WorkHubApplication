using IdentityService.BLL.DTOs;
using IdentityService.BLL.Services.BlobService;

namespace IdentityService.BLL.UseCases.FileUseCases.Queries.GetImageById;

public class GetImageByIdQueryHandler(IBlobService blobService) : IRequestHandler<GetImageByIdQuery, FileResponseDTO>
{
    public async Task<FileResponseDTO> Handle(GetImageByIdQuery request, CancellationToken cancellationToken)
    {
        var result = await blobService.DownloadAsync(request.Id, cancellationToken);

        return result;
    }
}

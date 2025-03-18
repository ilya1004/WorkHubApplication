using IdentityService.BLL.DTOs;

namespace IdentityService.BLL.UseCases.FileUseCases.Queries.GetImageByUserId;

public sealed record GetImageByUserIdQuery(Guid UserId) : IRequest<FileResponseDto>;
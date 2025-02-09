using IdentityService.BLL.DTOs;

namespace IdentityService.BLL.UseCases.UserUseCases.Queries.GetImageByUserId;

public sealed record GetImageByUserIdQuery(Guid UserId) : IRequest<FileResponseDTO>;
using IdentityService.BLL.DTOs;

namespace IdentityService.BLL.UseCases.FileUseCases.Queries.GetImageById;

public sealed record GetImageByIdQuery(Guid Id) : IRequest<FileResponseDto>;
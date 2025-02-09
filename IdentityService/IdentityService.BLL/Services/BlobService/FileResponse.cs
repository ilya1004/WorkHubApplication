namespace IdentityService.BLL.Services.BlobService;

public sealed record FileResponse(Stream Stream, string ContentType);
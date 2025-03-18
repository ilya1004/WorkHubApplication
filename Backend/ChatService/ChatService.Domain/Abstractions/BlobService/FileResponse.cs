namespace ChatService.Domain.Abstractions.BlobService;

public record FileResponse(Stream Stream, string ContentType);
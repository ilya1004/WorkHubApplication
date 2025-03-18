namespace ChatService.API.Contracts.ChatContracts;

public sealed record CreateFileMessageRequest(Guid ChatId, Guid ReceiverId, IFormFile File);
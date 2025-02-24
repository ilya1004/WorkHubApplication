namespace ChatService.API.Contracts.ChatContracts;

public sealed record SendFileMessageRequest(
    Guid ChatId,
    Guid ReceiverId,
    IFormFile File);
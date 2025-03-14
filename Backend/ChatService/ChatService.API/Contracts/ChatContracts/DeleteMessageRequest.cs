namespace ChatService.API.Contracts.ChatContracts;

public sealed record DeleteMessageRequest(Guid ReceiverId, Guid MessageId);
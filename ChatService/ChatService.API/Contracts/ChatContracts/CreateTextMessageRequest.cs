namespace ChatService.API.Contracts.ChatContracts;

public sealed record CreateTextMessageRequest(Guid ChatId, Guid ReceiverId, string Text);
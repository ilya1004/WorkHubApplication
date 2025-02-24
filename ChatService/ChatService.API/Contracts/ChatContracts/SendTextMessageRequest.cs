namespace ChatService.API.Contracts.ChatContracts;

public sealed record SendTextMessageRequest(
    Guid ChatId,
    Guid ReceiverId,
    string Text);
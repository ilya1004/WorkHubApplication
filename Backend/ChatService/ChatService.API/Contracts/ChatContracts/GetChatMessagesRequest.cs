namespace ChatService.API.Contracts.ChatContracts;

public sealed record GetChatMessagesRequest(Guid ChatId, int PageNo, int PageSize);
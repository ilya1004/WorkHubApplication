namespace ChatService.API.Contracts.ChatContracts;

public sealed record GetAllChatsRequest(int PageNo = 1, int PageSize = 10);
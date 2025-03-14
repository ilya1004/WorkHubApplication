using ChatService.Applications.Models;

namespace ChatService.Applications.UseCases.ChatUseCases.Queries.GetAllChats;

public sealed record GetAllChatsQuery(int PageNo, int PageSize) : IRequest<PaginatedResultModel<Chat>>;
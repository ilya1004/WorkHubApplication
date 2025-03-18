using ChatService.Application.Models;

namespace ChatService.Application.UseCases.ChatUseCases.Queries.GetAllChats;

public sealed record GetAllChatsQuery(int PageNo, int PageSize) : IRequest<PaginatedResultModel<Chat>>;
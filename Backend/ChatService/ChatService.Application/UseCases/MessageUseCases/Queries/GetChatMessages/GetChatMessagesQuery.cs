using ChatService.Application.Models;

namespace ChatService.Application.UseCases.MessageUseCases.Queries.GetChatMessages;

public sealed record GetChatMessagesQuery(Guid ChatId, int PageNo, int PageSize) : IRequest<PaginatedResultModel<Message>>;
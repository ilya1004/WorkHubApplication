using ChatService.Applications.Models;

namespace ChatService.Applications.UseCases.MessageUseCases.GetChatMessages;

public sealed record GetChatMessagesQuery(Guid ChatId, int PageNo, int PageSize) : IRequest<PaginatedResultModel<Message>>;
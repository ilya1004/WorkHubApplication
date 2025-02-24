using ChatService.Applications.Models;
using ChatService.Domain.Entities;

namespace ChatService.API.HubInterfaces;

public interface IChatClient
{
    Task ReceiveTextMessage(string text);
    Task ReceiveFileMessage(Guid fileId);
    Task ReceiveChatMessages(PaginatedResultModel<Message> messages);
    Task MessageIsDeleted(Guid messageId);
}
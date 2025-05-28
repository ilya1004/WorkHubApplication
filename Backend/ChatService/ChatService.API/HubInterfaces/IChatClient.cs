using ChatService.Application.Models;
using ChatService.Domain.Entities;

namespace ChatService.API.HubInterfaces;

public interface IChatClient
{
    Task ReceiveChat(Chat? chat);
    Task ReceiveTextMessage(Message message);
    Task ReceiveFileMessage(Message message);
    Task ReceiveChatMessages(PaginatedResultModel<Message> messages);
    Task MessageIsDeleted(Guid messageId);
}
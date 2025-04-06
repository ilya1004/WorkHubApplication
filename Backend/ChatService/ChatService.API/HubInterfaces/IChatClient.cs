using ChatService.Application.Models;
using ChatService.Domain.Entities;

namespace ChatService.API.HubInterfaces;

public interface IChatClient
{
    Task ReceiveChat(Chat? chat);
    Task ChatNotFound(Guid projectId);
    Task ReceiveTextMessage(Message message);
    Task ReceiveFileMessage(Message message);
    Task ReceiveChatMessages(PaginatedResultModel<Message> messages);
    Task ReceiveAllChats(PaginatedResultModel<Chat> chats);
    Task MessageIsDeleted(Guid messageId);
}
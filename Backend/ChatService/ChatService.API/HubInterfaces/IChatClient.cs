using ChatService.Application.Models;
using ChatService.Domain.Entities;

namespace ChatService.API.HubInterfaces;

public interface IChatClient
{
    Task ReceiveTextMessage(string text);
    Task ReceiveFileMessage(Guid fileId);
    Task ReceiveChatMessages(PaginatedResultModel<Message> messages);
    Task ReceiveAllChats(PaginatedResultModel<Chat> chats);
    Task MessageIsDeleted(Guid messageId);
}
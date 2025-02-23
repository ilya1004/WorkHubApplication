using ChatService.Domain.Entities;

namespace ChatService.Domain.Abstractions.Repositories;

public interface IUnitOfWork
{
    IMessagesRepository MessagesRepository { get; }
    IRepository<Chat> ChatRepository { get; }
}
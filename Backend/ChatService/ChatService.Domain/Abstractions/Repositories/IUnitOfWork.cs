namespace ChatService.Domain.Abstractions.Repositories;

public interface IUnitOfWork
{
    IMessagesRepository MessagesRepository { get; }
    IChatsRepository ChatRepository { get; }
}
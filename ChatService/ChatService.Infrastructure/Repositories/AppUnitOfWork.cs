using ChatService.Domain.Abstractions.Repositories;
using ChatService.Infrastructure.Constants;
using ChatService.Infrastructure.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace ChatService.Infrastructure.Repositories;

public class AppUnitOfWork : IUnitOfWork
{
    private readonly Lazy<IMessagesRepository> _messages;
    private readonly Lazy<IChatsRepository> _chats;

    public AppUnitOfWork(IMongoClient client, IOptions<MongoDbSettings> options)
    {
        var database = client.GetDatabase(options.Value.DatabaseName);

        _messages = new Lazy<IMessagesRepository>(() => new MessagesRepository(database));
        _chats = new Lazy<IChatsRepository>(() => new ChatsRepository(database));
    }

    public IMessagesRepository MessagesRepository => _messages.Value;
    public IChatsRepository ChatRepository => _chats.Value;
    
}
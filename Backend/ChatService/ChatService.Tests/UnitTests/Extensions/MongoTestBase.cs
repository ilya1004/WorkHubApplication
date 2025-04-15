using Mongo2Go;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

namespace ChatService.Tests.UnitTests.Extensions;

public abstract class MongoTestBase : IDisposable
{
    protected readonly MongoDbRunner Runner;
    protected readonly IMongoDatabase Database;
    protected readonly IMongoClient Client;

    protected MongoTestBase()
    {
        Runner = MongoDbRunner.Start();
        Client = new MongoClient(Runner.ConnectionString);
        Database = Client.GetDatabase("TestDatabase");

        // Apply global GuidSerializer configuration
        // if (!BsonSerializer.IsSerializerRegistered(typeof(Guid)))
        // {
            // BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));
        // }

        // Configure BsonClassMap for Chat
        if (!BsonClassMap.IsClassMapRegistered(typeof(Chat)))
        {
            BsonClassMap.RegisterClassMap<Chat>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
                cm.MapIdMember(c => c.Id)
                    .SetIdGenerator(GuidGenerator.Instance)
                    .SetSerializer(new GuidSerializer(GuidRepresentation.Standard));
                cm.MapMember(c => c.CreatedAt)
                    .SetDefaultValue(() => DateTime.UtcNow)
                    .SetSerializer(new DateTimeSerializer(BsonType.DateTime));
            });
        }

        // Configure BsonClassMap for Message
        if (!BsonClassMap.IsClassMapRegistered(typeof(Message)))
        {
            BsonClassMap.RegisterClassMap<Message>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
                cm.MapIdMember(m => m.Id)
                    .SetIdGenerator(GuidGenerator.Instance)
                    .SetSerializer(new GuidSerializer(GuidRepresentation.Standard));
                cm.MapMember(m => m.CreatedAt)
                    .SetDefaultValue(() => DateTime.UtcNow)
                    .SetSerializer(new DateTimeSerializer(BsonType.DateTime));
                cm.MapMember(m => m.FileId).SetIgnoreIfNull(true);
                cm.MapMember(m => m.Text).SetIgnoreIfNull(true);
            });
        }
    }

    public void Dispose()
    {
        Runner.Dispose();
    }
}
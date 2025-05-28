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

    static MongoTestBase()
    {
        try
        {
            BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));
        }
        catch (BsonSerializationException)
        {

        }

        if (!BsonClassMap.IsClassMapRegistered(typeof(Chat)))
        {
            BsonClassMap.RegisterClassMap<Chat>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
                cm.MapIdMember(c => c.Id)
                    .SetIdGenerator(GuidGenerator.Instance)
                    .SetSerializer(new GuidSerializer(GuidRepresentation.Standard));
                cm.MapMember(c => c.EmployerId)
                    .SetSerializer(new GuidSerializer(GuidRepresentation.Standard));
                cm.MapMember(c => c.FreelancerId)
                    .SetSerializer(new GuidSerializer(GuidRepresentation.Standard));
                cm.MapMember(c => c.ProjectId)
                    .SetSerializer(new GuidSerializer(GuidRepresentation.Standard));
                cm.MapMember(c => c.CreatedAt)
                    .SetDefaultValue(() => DateTime.UtcNow)
                    .SetSerializer(new DateTimeSerializer(BsonType.DateTime));
            });
        }

        if (!BsonClassMap.IsClassMapRegistered(typeof(Message)))
        {
            BsonClassMap.RegisterClassMap<Message>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
                cm.MapIdMember(m => m.Id)
                    .SetIdGenerator(GuidGenerator.Instance)
                    .SetSerializer(new GuidSerializer(GuidRepresentation.Standard));
                cm.MapMember(m => m.ChatId)
                    .SetSerializer(new GuidSerializer(GuidRepresentation.Standard));
                cm.MapMember(m => m.SenderId)
                    .SetSerializer(new GuidSerializer(GuidRepresentation.Standard));
                cm.MapMember(m => m.ReceiverId)
                    .SetSerializer(new GuidSerializer(GuidRepresentation.Standard));
                cm.MapMember(m => m.FileId)
                    .SetIgnoreIfNull(true);
                cm.MapMember(m => m.Text)
                    .SetIgnoreIfNull(true);
                cm.MapMember(m => m.CreatedAt)
                    .SetDefaultValue(() => DateTime.UtcNow)
                    .SetSerializer(new DateTimeSerializer(BsonType.DateTime));
            });
        }
    }

    protected MongoTestBase()
    {
        Runner = MongoDbRunner.Start();
        Client = new MongoClient(Runner.ConnectionString);
        Database = Client.GetDatabase("TestDatabase");
    }

    public void Dispose()
    {
        Runner.Dispose();
    }
}
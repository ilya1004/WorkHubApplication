using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;

namespace ChatService.Infrastructure.Configurations;

public static class ChatConfiguration
{
    public static void Configure()
    {
        if (!BsonClassMap.IsClassMapRegistered(typeof(Chat)))
        {
            BsonClassMap.RegisterClassMap<Chat>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
                
                cm.SetDiscriminator(nameof(Chat));

                cm.MapIdMember(c => c.Id)
                    .SetIdGenerator(GuidGenerator.Instance)
                    .SetSerializer(new GuidSerializer(BsonType.String));

                cm.MapMember(c => c.CreatedAt)
                    .SetDefaultValue(() => DateTime.UtcNow)
                    .SetSerializer(new DateTimeSerializer(BsonType.DateTime));
            });
        }
    }
}
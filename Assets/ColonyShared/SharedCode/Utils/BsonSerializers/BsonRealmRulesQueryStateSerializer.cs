using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using SharedCode.Entities;

namespace SharedCode.Utils.BsonSerialization
{
    public class BsonRealmRulesQueryStateSerializer : SerializerBase<RealmRulesQueryState>
    {
        public static readonly int Version = 1;

        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, RealmRulesQueryState value)
        {
            context.Writer.WriteStartDocument();
            context.Writer.WriteBoolean(nameof(RealmRulesQueryState.Available), value.Available);
            context.Writer.WriteEndDocument();
        }

        public override RealmRulesQueryState Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            context.Reader.ReadStartDocument();
            var available = context.Reader.ReadBoolean(nameof(RealmRulesQueryState.Available));
            context.Reader.ReadEndDocument();
            return new RealmRulesQueryState(available);
        }
    }
}
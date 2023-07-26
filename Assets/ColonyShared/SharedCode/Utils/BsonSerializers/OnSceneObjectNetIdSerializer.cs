using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using SharedCode.Entities;

namespace SharedCode.Utils.BsonSerialization
{
    public class BsonOnSceneObjectNetIdSerializer : SerializerBase<OnSceneObjectNetId>
    {
        public static readonly int Version = 1;

        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, OnSceneObjectNetId value)
        {
            context.Writer.WriteStartDocument();
            context.Writer.WriteBoolean("IsValid", value.IsValid);
            context.Writer.WriteInt32("SceneHash", value.SceneHash);
            context.Writer.WriteInt32("IndexInList", value.IndexInList);
            context.Writer.WriteEndDocument();
        }

        public override OnSceneObjectNetId Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            context.Reader.ReadStartDocument();
            bool IsValid = context.Reader.ReadBoolean("IsValid");
            int SceneHash = context.Reader.ReadInt32("SceneHash");
            int IndexInList = context.Reader.ReadInt32("IndexInList");
            context.Reader.ReadEndDocument();
            return new OnSceneObjectNetId(IsValid, SceneHash, IndexInList);
        }
    }
}
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace SharedCode.Utils.BsonSerialization
{

    public class BsonVector2IntSerializer : SerializerBase<Vector2Int>
    {
        public static readonly int Version = 1;

        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, Vector2Int value)
        {
            context.Writer.WriteStartDocument();
            context.Writer.WriteInt32("x", value.x);
            context.Writer.WriteInt32("y", value.y);
            context.Writer.WriteEndDocument();
        }

        public override Vector2Int Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            context.Reader.ReadStartDocument();
            int x = context.Reader.ReadInt32("x");
            int y = context.Reader.ReadInt32("y");
            context.Reader.ReadEndDocument();
            return new Vector2Int(x, y);
        }
    }
}

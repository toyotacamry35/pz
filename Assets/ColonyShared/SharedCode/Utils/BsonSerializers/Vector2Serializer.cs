using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace SharedCode.Utils.BsonSerialization
{
    public class BsonVector2Serializer : SerializerBase<Vector2>
    {
        public static readonly int Version = 1;

        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, Vector2 value)
        {
            context.Writer.WriteStartDocument();
            context.Writer.WriteDouble("x", value.x);
            context.Writer.WriteDouble("y", value.y);
            context.Writer.WriteEndDocument();
        }

        public override Vector2 Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            context.Reader.ReadStartDocument();
            float x = (float)context.Reader.ReadDouble("x");
            float y = (float)context.Reader.ReadDouble("y");
            context.Reader.ReadEndDocument();
            return new Vector2(x, y);
        }
    }
}

using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace SharedCode.Utils.BsonSerialization
{

    public class BsonQuaternionSerializer : SerializerBase<Quaternion>
    {
        public static readonly int Version = 1;

        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, Quaternion value)
        {
            context.Writer.WriteStartDocument();
            context.Writer.WriteDouble("x", value.x);
            context.Writer.WriteDouble("y", value.y);
            context.Writer.WriteDouble("z", value.z);
            context.Writer.WriteDouble("w", value.w);
            context.Writer.WriteEndDocument();
        }

        public override Quaternion Deserialize(BsonDeserializationContext context, MongoDB.Bson.Serialization.BsonDeserializationArgs args)
        {
            context.Reader.ReadStartDocument();
            float x = (float)context.Reader.ReadDouble("x");
            float y = (float)context.Reader.ReadDouble("y");
            float z = (float)context.Reader.ReadDouble("z");
            float w = (float)context.Reader.ReadDouble("w");
            context.Reader.ReadEndDocument();
            return new Quaternion(x, y, z, w);
        }
    }
}

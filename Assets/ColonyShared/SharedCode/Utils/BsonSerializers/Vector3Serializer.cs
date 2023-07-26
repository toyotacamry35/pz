using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace SharedCode.Utils.BsonSerialization
{

    public class BsonVector3Serializer : SerializerBase<Vector3>
    {
        public static readonly int Version = 1;

        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, Vector3 value)
        {
            context.Writer.WriteStartDocument();
            context.Writer.WriteDouble("x", value.x);
            context.Writer.WriteDouble("y", value.y);
            context.Writer.WriteDouble("z", value.z);
            context.Writer.WriteEndDocument();
        }

        public override Vector3 Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            context.Reader.ReadStartDocument();
            float x = (float)context.Reader.ReadDouble("x");
            float y = (float)context.Reader.ReadDouble("y");
            float z = (float)context.Reader.ReadDouble("z");
            context.Reader.ReadEndDocument();
            return new Vector3(x, y, z);
        }
    }
}

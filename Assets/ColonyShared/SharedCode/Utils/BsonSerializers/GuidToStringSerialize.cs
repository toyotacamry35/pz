using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using System;

namespace SharedCode.Utils.BsonSerialization
{
    public class StringGuidSerializer : SerializerBase<Guid>
    {
        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, Guid value)
        {
            context.Writer.WriteString(value.ToString());
        }

        public override Guid Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            return new Guid(context.Reader.ReadString());
        }
    }
}

using System;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace SharedCode.EntitySystem
{
    public class BsonOuterRefSerializer<TValue> : SerializerBase<OuterRef<TValue>>
    {
        public static readonly int Version = 1;

        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, OuterRef<TValue> value)
        {
            context.Writer.WriteStartDocument();
            context.Writer.WriteBytes("Guid", value.Guid.ToByteArray());
            context.Writer.WriteInt32("TypeId", value.TypeId);
            context.Writer.WriteEndDocument();
        }

        public override OuterRef<TValue> Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            context.Reader.ReadStartDocument();
            byte[] Guid = context.Reader.ReadBytes("Guid");
            int TypeId = context.Reader.ReadInt32("TypeId");
            context.Reader.ReadEndDocument();
            return new OuterRef<TValue>(new Guid(Guid), TypeId);
        }
    }
}

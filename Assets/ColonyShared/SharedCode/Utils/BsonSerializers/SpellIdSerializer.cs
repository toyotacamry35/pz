using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using SharedCode.Wizardry;

namespace SharedCode.Utils.BsonSerialization
{
    public class BsonSpellIdSerializer : SerializerBase<SpellId>
    {
        public static readonly int Version = 1;

        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, SpellId value)
        {
            context.Writer.WriteStartDocument();
            context.Writer.WriteInt64("Value", (long)value.Counter);
            context.Writer.WriteEndDocument();
        }

        public override SpellId Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            context.Reader.ReadStartDocument();
            ulong Value = (ulong)context.Reader.ReadInt64("Value");
            context.Reader.ReadEndDocument();
            return new SpellId(Value);
        }
    }
}
using Assets.ColonyShared.SharedCode.Aspects.Damage.Templates;
using Assets.Src.Aspects.Impl.Stats;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace SharedCode.Utils.BsonSerialization
{
    public class BsonStatModifierSerializer : SerializerBase<StatModifier>
    {
        public static readonly int Version = 1;
        private BsonIResourceSerializer<StatResource> bsonBaseResourceSerializer = new BsonIResourceSerializer<StatResource>();

        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, StatModifier value)
        {
            context.Writer.WriteStartDocument();
            context.Writer.WriteName("Stat");
            bsonBaseResourceSerializer.Serialize(context, value.Stat);
            context.Writer.WriteDouble("Value", value.Value);
            context.Writer.WriteEndDocument();
        }

        public override StatModifier Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            context.Reader.ReadStartDocument();
            string StatName = context.Reader.ReadName();
            StatResource stat = bsonBaseResourceSerializer.Deserialize(context);
            float Value = (float)context.Reader.ReadDouble("Value");
            context.Reader.ReadEndDocument();
            StatModifier ret = new StatModifier(stat, Value);
            return ret;
        }
    }
}
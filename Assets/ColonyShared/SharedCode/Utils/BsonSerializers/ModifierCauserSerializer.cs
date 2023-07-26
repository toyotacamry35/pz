using Assets.Src.ResourcesSystem.Base;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using SharedCode.Entities.Engine;

namespace SharedCode.Utils.BsonSerialization
{

    public class BsonModifierCauserSerializer : SerializerBase<ModifierCauser>
    {
        public static readonly int Version = 1;

        private BsonIResourceSerializer<SaveableBaseResource> bsonBaseResourceSerializer = new BsonIResourceSerializer<SaveableBaseResource>();

        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, ModifierCauser value)
        {
            context.Writer.WriteStartDocument();
            context.Writer.WriteName("Causer");
            bsonBaseResourceSerializer.Serialize(context, value.Causer);
            context.Writer.WriteInt64("SpellId", (long)value.SpellId);
            context.Writer.WriteEndDocument();
        }

        public override ModifierCauser Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            context.Reader.ReadStartDocument();
            string CauserName = context.Reader.ReadName();
            BaseResource Causer = bsonBaseResourceSerializer.Deserialize(context);
            ulong SpellId = (ulong)context.Reader.ReadInt64("SpellId");
            context.Reader.ReadEndDocument();
            return new ModifierCauser(Causer, SpellId);
        }
    }

    public class OuterRefSerializer : SerializerBase<ResourceSystem.Utils.OuterRef>
    {
        public static readonly int Version = 1;

        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, ResourceSystem.Utils.OuterRef value)
        {
            context.Writer.WriteStartDocument();
            context.Writer.WriteInt32("TypeId", value.TypeId);
            context.Writer.WriteBytes("EntityId", value.Guid.ToByteArray());
            context.Writer.WriteEndDocument();
        }

        public override ResourceSystem.Utils.OuterRef Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            context.Reader.ReadStartDocument();
            int typeId = (int)context.Reader.ReadInt32("TypeId");
            var id = new System.Guid(context.Reader.ReadBytes("EntityId"));
            context.Reader.ReadEndDocument();
            return new ResourceSystem.Utils.OuterRef(id, typeId);
        }
    }
}

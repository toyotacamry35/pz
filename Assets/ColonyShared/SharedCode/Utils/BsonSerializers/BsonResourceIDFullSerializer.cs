using Assets.Src.ResourcesSystem.Base;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace SharedCode.Utils.BsonSerialization
{
    public class BsonResourceIDFullSerializer : SerializerBase<ResourceIDFull>
    {
        public static readonly int Version = 1;

        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, ResourceIDFull value)
        {
            context.Writer.WriteStartDocument();
            context.Writer.WriteString("Root", value.Root != null ? value.Root : "");
            context.Writer.WriteInt32("Line", value.Line);
            context.Writer.WriteInt32("Col", value.Col);
            context.Writer.WriteInt32("ProtoIndex", value.ProtoIndex);
            context.Writer.WriteEndDocument();
        }

        public override ResourceIDFull Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            context.Reader.ReadStartDocument();
            string Root = context.Reader.ReadString("Root");
            int Line = context.Reader.ReadInt32("Line");
            int Col = context.Reader.ReadInt32("Col");
            int ProtoIndex = context.Reader.ReadInt32("ProtoIndex");
            context.Reader.ReadEndDocument();
            return new ResourceIDFull(string.IsNullOrEmpty(Root) ? null : Root, Line, Col, ProtoIndex);
        }
    }
}
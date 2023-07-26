using System;
using GeneratedCode.MapSystem;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using SharedCode.Utils;
using SharedCode.Utils.BsonSerialization;

namespace SharedCode.EntitySystem
{
    public class SceneLoadableObjectsDataSerializer : SerializerBase<SceneLoadableObjectsData>
    {
        public static readonly int Version = 1;
        static BsonVector3Serializer _vec = new BsonVector3Serializer();
        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, SceneLoadableObjectsData value)
        {
            context.Writer.WriteStartDocument();
            context.Writer.WriteBytes(nameof(value.StaticId), value.StaticId.ToByteArray());
            context.Writer.WriteName(nameof(value.WorldSpacePosId));
            _vec.Serialize(context, value.WorldSpacePosId);
            context.Writer.WriteEndDocument();
        }

        public override SceneLoadableObjectsData Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            context.Reader.ReadStartDocument();
            byte[] Guid = context.Reader.ReadBytes(nameof(SceneLoadableObjectsData.StaticId));
            context.Reader.ReadName();
            var vec = (Vector3)_vec.Deserialize(context);
            context.Reader.ReadEndDocument();
            return new SceneLoadableObjectsData() { StaticId = new System.Guid(Guid), WorldSpacePosId = vec };
        }
    }
}

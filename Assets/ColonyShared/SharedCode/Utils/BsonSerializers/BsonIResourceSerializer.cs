using Assets.Src.ResourcesSystem.Base;
using ResourcesSystem.Loader;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using System;

namespace SharedCode.Utils.BsonSerialization
{
    public class IResourcesBsonSerializationProvider : IBsonSerializationProvider
    {
        public IBsonSerializer GetSerializer(Type type)
        {
            if (typeof(ISaveableResource).IsAssignableFrom(type))
            {
                var gtype = typeof(BsonIResourceSerializer<>).MakeGenericType(type);
                var obj = (IBsonSerializer)Activator.CreateInstance(gtype);
                return obj;
            }
            else
            {
                return null;
            }
        }
    }

    public class BsonIResourceSerializer<TValue> : SerializerBase<TValue> where TValue : ISaveableResource
    {
        public static readonly int Version = 1;

        private static ulong DefaultResourceIDFullCrc = default(ResourceIDFull).RootID();

        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, TValue value)
        {
            var saveable = (ISaveableResource)value;
            
            context.Writer.WriteStartDocument();
            if(value == null)
                context.Writer.WriteBytes("SaveableId", Guid.Empty.ToByteArray());
            else
                context.Writer.WriteBytes("SaveableId", saveable.Id.ToByteArray());
            context.Writer.WriteEndDocument();
        }

        public override TValue Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            context.Reader.ReadStartDocument();
            var name = context.Reader.ReadName();
            if (name != "SaveableId")
            {
                ulong ResourceID = (ulong)context.Reader.ReadInt64();
                int Col = context.Reader.ReadInt32("Col");
                int Line = context.Reader.ReadInt32("Line");
                int ProtoIndex = context.Reader.ReadInt32("ProtoIndex");
                context.Reader.ReadEndDocument();
                if (ResourceID == DefaultResourceIDFullCrc)
                    return default(TValue);
                var id = GameResourcesHolder.Instance.NetIDs.GetID(ResourceID, Line, Col, ProtoIndex);
                return GameResourcesHolder.Instance.LoadResource<TValue>(id);
            }
            else
            {
                Guid resourceId = new Guid(context.Reader.ReadBytes());
                context.Reader.ReadEndDocument();

                if (resourceId == Guid.Empty)
                    return default;
                var resource = GameResourcesHolder.Instance.SaveableStorage.GetResourceById(resourceId);
                return (TValue)resource;
            }
        }
    }
}
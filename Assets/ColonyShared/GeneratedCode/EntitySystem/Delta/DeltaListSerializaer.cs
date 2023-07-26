using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using System.Collections.Generic;

namespace SharedCode.EntitySystem.Delta
{

    public class BsonDeltaListSerializer<TValue> : SerializerBase<IDeltaList<TValue>>
    {
        public static readonly int Version = 1;

        private readonly EnumerableInterfaceImplementerSerializer<List<TValue> ,TValue> _rawSerializer;
        private readonly EnumerableInterfaceImplementerSerializer<List<ulong?>,ulong?> _deltaObjectsIdsSerializer;

        public BsonDeltaListSerializer()
        {
            _rawSerializer = new EnumerableInterfaceImplementerSerializer<List<TValue>, TValue>();
            _deltaObjectsIdsSerializer = new EnumerableInterfaceImplementerSerializer<List<ulong?>,ulong?>();
        }

        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, IDeltaList<TValue> value)
        {
            context.Writer.WriteStartDocument();
            var list = (DeltaList<TValue>)value;
            context.Writer.WriteInt64("LocalId", (long)list.LocalId);
            context.Writer.WriteName("Items");
            if (DeltaList<TValue>.IsDeltaObject)
            {
                _deltaObjectsIdsSerializer.Serialize(context, ((IDeltaListExt<TValue>) list).GetDeltaObjectsLocalIds());
            }
            else
            {
                _rawSerializer.Serialize(context, list.Items);
            }
           
            context.Writer.WriteEndDocument();
        }

        public override IDeltaList<TValue> Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            context.Reader.ReadStartDocument();
            var name = context.Reader.ReadName();
            // была полиморфная сериализация
            if (name == "_t")
            {
                // type
                context.Reader.ReadString();
                // _v
                context.Reader.ReadName();
                context.Reader.ReadStartDocument();
                return ReadList(context);
            }
            else
            {
                return ReadList(context);
            }
        }

        private IDeltaList<TValue> ReadList(BsonDeserializationContext context)
        {
            var localId = (ulong)context.Reader.ReadInt64();
            context.Reader.ReadName();

            DeltaList<TValue> list;
            if (DeltaList<TValue>.IsDeltaObject)
            {
                list = DeltaList<TValue>.CreateFromIds(_deltaObjectsIdsSerializer.Deserialize(context));
            }
            else
            {
                list = DeltaList<TValue>.CreateFromRawObjects(_rawSerializer.Deserialize(context));
            }
            
            context.Reader.ReadEndDocument();
            list.LocalId = localId;
            return list;
        }
    }
}

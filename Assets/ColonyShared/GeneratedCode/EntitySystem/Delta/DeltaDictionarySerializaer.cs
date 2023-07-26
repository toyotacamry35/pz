using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Options;
using MongoDB.Bson.Serialization.Serializers;
using System;
using System.Collections.Generic;

namespace SharedCode.EntitySystem.Delta
{
    public class BsonDeltaDictionarySerializer<TKey, TValue> : SerializerBase<IDeltaDictionary<TKey, TValue>>
    {
        public static readonly int Version = 1;

        private readonly DictionaryInterfaceImplementerSerializer<Dictionary<TKey, TValue>, TKey, TValue> _rawSerializer;
        private readonly DictionaryInterfaceImplementerSerializer<Dictionary<TKey, ulong?>, TKey, ulong?> _deltaObjectsIdsSerializer;

        public BsonDeltaDictionarySerializer()
        {
            DictionaryRepresentation represenation = typeof(TKey) == typeof(string)
                ? DictionaryRepresentation.Document
                : DictionaryRepresentation.ArrayOfArrays;
            _rawSerializer = new DictionaryInterfaceImplementerSerializer<Dictionary<TKey, TValue>, TKey, TValue>(represenation);
            _deltaObjectsIdsSerializer = new DictionaryInterfaceImplementerSerializer<Dictionary<TKey, ulong?>, TKey, ulong?>(represenation);
        }

        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, IDeltaDictionary<TKey, TValue> value)
        {
            context.Writer.WriteStartDocument();
            var dict = (DeltaDictionary<TKey, TValue>)value;
            context.Writer.WriteInt64("LocalId", (long)dict.LocalId);
            context.Writer.WriteName("Items");
            if (DeltaDictionary<TKey, TValue>.IsDeltaObject)
            {
                _deltaObjectsIdsSerializer.Serialize(context, ((IDeltaDictionaryExt<TKey, TValue>)dict).GetDeltaObjectsLocalIds());
            }
            else
            {
                _rawSerializer.Serialize(context, dict.Items);
            }
            
            context.Writer.WriteEndDocument();
        }

        public override IDeltaDictionary<TKey, TValue> Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
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
                return ReadDictionary(context);
            }
            else
            {
                return ReadDictionary(context);
            }
        }

        private IDeltaDictionary<TKey, TValue> ReadDictionary(BsonDeserializationContext context)
        {
            var localId = (ulong)context.Reader.ReadInt64();
            context.Reader.ReadName();
            DeltaDictionary<TKey, TValue> dict;
            if (DeltaDictionary<TKey, TValue>.IsDeltaObject)
            {
                dict = DeltaDictionary<TKey, TValue>.CreateFromIds(_deltaObjectsIdsSerializer.Deserialize(context));
            }
            else
            {
                dict = DeltaDictionary<TKey, TValue>.CreateFromRawObjects(_rawSerializer.Deserialize(context));
            }
            
            context.Reader.ReadEndDocument();
            dict.LocalId = localId;
            return dict;
        }
    }
}

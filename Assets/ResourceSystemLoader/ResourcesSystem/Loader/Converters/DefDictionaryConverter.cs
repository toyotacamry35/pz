using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Src.ResourcesSystem.Base;
using Newtonsoft.Json;
using NLog;

namespace ResourcesSystem.Loader
{
    public class DefDictionaryConverter : JsonConverter
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();
        
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            while (reader.TokenType == JsonToken.Comment)
                reader.Read();
            
            var context = (LoadingContext)serializer.Context.Context;
            var lineInfo = (JsonTextReader)reader;

            if (reader.TokenType != JsonToken.StartObject)
                throw new Exception($"Expecting {nameof(JsonToken.StartObject)} token at {context.RootAddress}:{lineInfo.LineNumber}:{lineInfo.LinePosition}");
            var typeToGetKeys = objectType.IsGenericType ? objectType : objectType.BaseType;
            var keyType = typeToGetKeys.GenericTypeArguments[0];
            var valueType = typeToGetKeys.GenericTypeArguments[1];

            var dictionary = Activator.CreateInstance(objectType) as IDictionary;
            if(dictionary == null)
                throw new Exception($"Can't create dictionary with type {objectType}");
            
            while(true)
            {
                ReadToNext(reader);
                if (reader.TokenType == JsonToken.EndObject)
                    break;
                if (reader.TokenType != JsonToken.PropertyName)
                    throw new Exception($"Expecting {nameof(JsonToken.PropertyName)} token at {context.RootAddress}:{lineInfo.LineNumber}:{lineInfo.LinePosition}");
                var key = serializer.Deserialize(reader, keyType);
                ReadToNext(reader);
                var value = serializer.Deserialize(reader, valueType);
                dictionary.Add(key, value);
            }

            return dictionary;
        }

        private void ReadToNext(JsonReader reader)
        {
            do
            {
                reader.Read();
            } while (reader.TokenType == JsonToken.Comment);
        }
        
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
            }
            else
            {
                var dictionary = value as IDictionary;
                writer.WriteStartObject();
                foreach (DictionaryEntry kv in dictionary)
                {
                    writer.WritePropertyName(((IRefBase)kv.Key).TargetBase.Address.ToString());
                    serializer.Serialize(writer, kv.Value);
                }
                writer.WriteEndObject();
            }
        }

        public override bool CanConvert(Type objectType)
        {
            if (_canConvert.TryGetValue(objectType, out var canConvert))
                return canConvert;
            var result = _canConvert[objectType] = objectType.GetInterfaces().Any(x =>
                x.IsGenericType &&
                x.GetGenericTypeDefinition() == typeof(IDictionary<,>) &&
                x.GenericTypeArguments[0].IsGenericType &&
                x.GenericTypeArguments[0].GetGenericTypeDefinition() == typeof(ResourceRef<>) 
            );
            return result;
        }

        private readonly Dictionary<Type,bool> _canConvert = new Dictionary<Type, bool>();
    }
}
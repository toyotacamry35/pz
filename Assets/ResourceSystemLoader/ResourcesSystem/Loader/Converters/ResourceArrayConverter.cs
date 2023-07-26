using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Src.ResourcesSystem.Base;
using Newtonsoft.Json;

namespace ResourcesSystem.Loader
{
    public class ResourceArrayConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType.IsGenericType &&
                   objectType.GetGenericTypeDefinition() == typeof(ResourceArray<>);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            while (reader.TokenType == JsonToken.Comment)
                reader.Read();
            
            var context = (LoadingContext)serializer.Context.Context;
            var lineInfo = (JsonTextReader)reader;

            if (reader.TokenType != JsonToken.StartArray)
                GameResources.ThrowError<JsonException>($"Expecting {nameof(JsonToken.StartArray)} token at {context.RootAddress}:{lineInfo.LineNumber}:{lineInfo.LinePosition}");

            var valueType = objectType.GenericTypeArguments[0];
            var refType = typeof(ResourceRef<>).MakeGenericType(valueType);
            
            var list = new List<IRefBase>();
            while(true)
            {
                ReadToNext(reader);
                if (reader.TokenType == JsonToken.EndArray)
                    break;
                var value = (IRefBase)serializer.Deserialize(reader, refType);
                if(value.TargetBase != null && !valueType.IsInstanceOfType(value.TargetBase)) // да, теперь можно добавлять null в качестве элемента массива
                    GameResources.ThrowError<JsonException>($"Array element type {value.GetType()} is not {valueType} at {context.RootAddress}:{lineInfo.LineNumber}:{lineInfo.LinePosition} ");
                else
                    list.Add(value);
            }
            
            var array = Activator.CreateInstance(objectType, list);
            if (array == null)
                throw new Exception($"Can't create dictionary with type {objectType}");
            
            return array;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
            }
            else
            {
                var array = (IEnumerable)value;
                writer.WriteStartArray();
                foreach (IRefBase entry in array)
                    serializer.Serialize(writer, entry);
                writer.WriteEndArray();
            }
        }
        
        private void ReadToNext(JsonReader reader)
        {
            do
            {
                reader.Read();
            } while (reader.TokenType == JsonToken.Comment);
        }
    }
}
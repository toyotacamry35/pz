using System;
using Newtonsoft.Json;

namespace ResourcesSystem.Loader
{
    public class TypeConverter : JsonConverter
    {
        public static TypeConverter Instance { get; } = new TypeConverter();

        private TypeConverter() { }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(((Type)value).Name);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return serializer.SerializationBinder.BindToType(string.Empty, DefConverter.ExpandTypeName((string)reader.Value, objectType));
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Type);
        }
    }

}

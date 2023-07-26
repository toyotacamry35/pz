using System;
using Assets.Src.ResourcesSystem.Base;
using Newtonsoft.Json;

namespace Assets.ColonyShared.SharedCode.Utils
{
    public class DefRefSkipConverter : JsonConverter
    {
        public static readonly DefRefSkipConverter Instance = new DefRefSkipConverter();

        private DefRefSkipConverter()
        {
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType.IsGenericType && objectType.GetGenericTypeDefinition() == typeof(ResourceRef<>);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return null;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteNull();
        }
    }
}
using System;
using Assets.ColonyShared.SharedCode.ResourcesSystem.Base;
using Newtonsoft.Json;

namespace Assets.ColonyShared.SharedCode.Utils
{
    public class UnityRefSkipConverter : JsonConverter
    {
        public static readonly UnityRefSkipConverter Instance = new UnityRefSkipConverter();

        private UnityRefSkipConverter()
        {
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType.IsGenericType && objectType.GetGenericTypeDefinition() == typeof(UnityRef<>);
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
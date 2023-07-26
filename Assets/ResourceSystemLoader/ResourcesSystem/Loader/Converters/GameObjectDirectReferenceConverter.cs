using Newtonsoft.Json;
using System;

namespace ResourcesSystem.Loader
{
    public class GameObjectDirectReferenceConverter : JsonConverter
    {
        public static Type UnityObjectType;

        public override bool CanConvert(Type objectType)
        {
            if (objectType == null || UnityObjectType == null)
                return false;
            return objectType.IsSubclassOf(UnityObjectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return null;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue($"UnityEngine.Object - {value?.GetType().Name ?? "null"}");
        }
    }

}



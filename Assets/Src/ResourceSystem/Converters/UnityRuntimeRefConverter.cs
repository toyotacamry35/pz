using Assets.ColonyShared.SharedCode.ResourcesSystem.Base;
using Newtonsoft.Json;
using System;

namespace Assets.Src.ResourceSystem.Converters
{
    public class UnityRuntimeRefConverter : JsonConverter
    {
        public static readonly UnityRuntimeRefConverter Instance = new UnityRuntimeRefConverter();

        public override bool CanRead => true;

        public override bool CanWrite => true;

        private UnityRuntimeRefConverter() { }

        public override bool CanConvert(Type objectType)
        {
            return objectType.IsGenericType && objectType.GetGenericTypeDefinition() == typeof(UnityRef<>);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var targetType = objectType.GetGenericArguments()[0];
            var tgt = UnityRuntimeObjectConverter.ReadImpl(reader, targetType, existingValue, serializer);
            var retVal = Activator.CreateInstance(objectType, tgt);

            return retVal;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var p = ((IUnityRef)value).Path;
            if (p == null)
                writer.WriteToken(JsonToken.Null, null);
            else
                writer.WriteValue(p);
        }
    }
}

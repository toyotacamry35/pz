using Assets.ColonyShared.SharedCode.ResourcesSystem.Base;
using Newtonsoft.Json;
using System;

namespace Assets.Src.ResourceSystem.Editor.Converters
{
    public class UnityOnImportRefConverter : JsonConverter
    {
        public static readonly UnityOnImportRefConverter Instance = new UnityOnImportRefConverter();

        public override bool CanRead => true;

        public override bool CanWrite => false;

        private UnityOnImportRefConverter() { }

        public override bool CanConvert(Type objectType)
        {
            return objectType.IsGenericType && objectType.GetGenericTypeDefinition() == typeof(UnityRef<>);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var targetType = objectType.GetGenericArguments()[0];
            var tgt = ResourceSystem.Converters.UnityRuntimeObjectConverter.ReadImpl(reader, targetType, existingValue, serializer);
            var retVal = Activator.CreateInstance(objectType, tgt);

            return retVal;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}

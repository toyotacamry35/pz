using Assets.Src.ResourcesSystem.Base;
using Newtonsoft.Json;
using System;

namespace ResourcesSystem.Base
{
    public class TelemetryJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(IResource).IsAssignableFrom(objectType);
        }

        public override bool CanRead => false;

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var res = (IResource)value;
            writer.WriteValue(res.Address.ToString());
        }
    }
}

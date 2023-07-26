using Newtonsoft.Json;
using System;

namespace Assets.Src.ResourceSystem.Editor.Converters
{
    public class UnityOnImportObjectConverter : JsonConverter
    {
        public static readonly UnityOnImportObjectConverter Instance = new UnityOnImportObjectConverter();

        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public delegate void UnityRefFoundDelegate(string path, string hint, UnityEngine.Object obj);

        private UnityOnImportObjectConverter() { }

        public override bool CanConvert(Type objectType)
        {
            return typeof(UnityEngine.Object).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}

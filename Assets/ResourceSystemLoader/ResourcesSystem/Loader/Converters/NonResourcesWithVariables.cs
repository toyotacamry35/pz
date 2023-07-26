using System;
using System.Collections;
using Assets.Src.ResourcesSystem.Base;
using Newtonsoft.Json;

namespace ResourcesSystem.Loader
{
    internal class NonResourcesWithVariables : JsonConverter
    {
        public static NonResourcesWithVariables Instance { get; } = new NonResourcesWithVariables();
        private bool _doWork = true;
        private NonResourcesWithVariables() { }

        public override bool CanConvert(Type objectType)
        {
            if (!_doWork)
                return false;
            if (objectType == typeof(string))
                return true;
            if (objectType.IsPrimitive)
                return true;
            return false;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var context = (LoadingContext)serializer.Context.Context;

            if (reader.TokenType == JsonToken.String)
                if (((string)reader.Value).StartsWith("@"))
                {
                    Type t;
                    return PrimitiveTypesConverter.Convert(context.GetVar(((string)reader.Value).Substring(1), out t), objectType);
                }
            _doWork = false;
            try
            {
                var obj = serializer.Deserialize(reader, objectType);
                _doWork = true;
                return obj;
            }
            finally
            {
                _doWork = true;
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
        }
    }

}

using System;
using Assets.ColonyShared.SharedCode.ResourcesSystem.Base;
using Assets.Src.ResourcesSystem.Base;
using Newtonsoft.Json;

namespace ResourcesSystem.Loader
{
    class TemplateVariableConverter : JsonConverter
    {
        public static TemplateVariableConverter Instance { get; } = new TemplateVariableConverter();

        private TemplateVariableConverter() { }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(TemplateVariable);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType != JsonToken.StartObject)
                GameResources.ThrowError("Template variable does not start with StartObject");

            var startDepth = reader.Depth;
            var startPath = reader.Path;
            reader.Read();
            //skip $type
            if (reader.TokenType == JsonToken.PropertyName && (string)reader.Value == "$type")
            {
                reader.Read();
                reader.Read();
            }
            //skip Type property name
            reader.Read();
            var templateVariable = new TemplateVariable();
            Type type = serializer.SerializationBinder.BindToType(string.Empty, DefConverter.ExpandTypeName((string)reader.Value, objectType));
            templateVariable.Type = type;

            reader.Read();
            if (reader.TokenType != JsonToken.EndObject)
            {
                reader.Read();
                if (typeof(IResource).IsAssignableFrom(type))
                {
                    templateVariable.Type = typeof(ResourceRef<>).MakeGenericType(typeof(BaseResource));
                    if (reader.TokenType != JsonToken.Null)
                        templateVariable.Value = serializer.Deserialize(reader, typeof(ResourceRef<>).MakeGenericType(type));
                    else
                        templateVariable.Value = null;

                }
                else
                {
                    templateVariable.Type = type;
                    if (typeof(UnityEngine.Object).IsAssignableFrom(type))
                    {
                        var uRef = typeof(UnityRef<>).MakeGenericType(type);
                        templateVariable.Type = uRef;
                    }
                    templateVariable.Value = serializer.Deserialize(reader, templateVariable.Type);
                }

                //read last token of value
                reader.Read();
            }
            var endDepth = reader.Depth;
            if (endDepth != startDepth)
                GameResources.ThrowError($"Depth is different in tempVarConverter {startPath} {startDepth} {endDepth} {type.Name}");

            return templateVariable;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }

}

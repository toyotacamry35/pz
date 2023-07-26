using System;
using System.Reflection;
using Assets.Src.ResourcesSystem.Base;
using ResourcesSystem.Loader;
using L10n;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;

namespace Assets.ColonyShared.SharedCode.Utils
{
    public class EmbeddedOnlyDefRefConverter : JsonConverter
    {
        private static readonly NLog.Logger Logger = LogManager.GetLogger("Localization");

        public override bool CanConvert(Type objectType)
        {
            return objectType.IsGenericType && objectType.GetGenericTypeDefinition() == typeof(ResourceRef<>);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            while (reader.TokenType == JsonToken.Comment)
                reader.Read();
            var context = (LoadingContext) serializer.Context.Context;

            var tokenType = reader.TokenType;
            var readerValue = reader.Value;
            var lineInfo = reader as IJsonLineInfo;
            var defType = objectType.GetGenericArguments()[0];
            if (tokenType == JsonToken.Null ||
                tokenType == JsonToken.String)
            {
                return CreateRef(defType, null);
            }

            if (tokenType == JsonToken.StartObject)
            {
                //this is either a template or a def
                //does it make sense to write a template inside a file? 
                //Well, in principle, why not?
                if (context.IsProtoChildFile)
                {
                    context.ProtoStack.Peek().Embedded = true;
                }

                var obj = serializer.Deserialize(reader, defType);
                return CreateRef(defType, (IResource) obj);
            }

            if (tokenType == JsonToken.Boolean || tokenType == JsonToken.Float || tokenType == JsonToken.Integer || tokenType == JsonToken.String)
            {
                var attr = defType.GetCustomAttribute<CanBeCreatedFromAliasedPrimitiveAttribute>(inherit: true);
                if (attr != null)
                {
                    var objectOfDesiredType = PrimitiveTypesConverter.Convert(readerValue, attr.PrimitiveType);
                    var def = defType.GetMethod(attr.MethodName, BindingFlags.Static | BindingFlags.Public).Invoke(null, new[] {objectOfDesiredType});
                    return CreateRef(defType, (IResource) def);
                }
            }

            //todo make proper line address
            GameResources.ThrowError<JsonException>(
                $"Reference to definition is not a string or object {lineInfo.LineNumber} {lineInfo.LinePosition} {context.RootAddress} - is {tokenType} {readerValue}");

            return default(object);
        }

        //Сериализует ограниченную версию объекта, содержащую только интересующие нас данные
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            var refTarget = ((IRefBase) value).TargetBase;
            if (refTarget == null ||
                //Костыль: ходим по ссылкам только локалилизуемых ресурсов.
                //По хорошему надо бы изначально подгружать только данные, которые содержатся в файле, чем сделать следующее ограничение ненужным
                refTarget.GetType().GetCustomAttributes(typeof(LocalizedAttribute), false).Length == 0)
            {
                writer.WriteNull();
                return;
            }

            JObject.FromObject(refTarget, serializer).WriteTo(writer);
        }

        internal static object CreateRef(Type resType, IResource res)
        {
            var type = typeof(ResourceRef<>).MakeGenericType(resType);
            // ReSharper disable once PossibleNullReferenceException
            var retVal = type.GetConstructor(new[] {resType}).Invoke(new object[] {res});
            return retVal;
        }
    }
}
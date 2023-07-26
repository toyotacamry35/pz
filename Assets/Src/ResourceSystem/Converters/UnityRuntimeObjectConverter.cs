using Assets.ColonyShared.SharedCode.ResourcesSystem.Base;
using Assets.Src.ResourcesSystem.Base;
using ResourcesSystem.Loader;
using Newtonsoft.Json;
using NLog;
using NLog.Fluent;
using System;
using System.IO;
using Core.Environment.Logging.Extension;

namespace Assets.Src.ResourceSystem.Converters
{
    public class UnityRuntimeObjectConverter : JsonConverter
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();
        public static readonly UnityRuntimeObjectConverter Instance = new UnityRuntimeObjectConverter();

        public delegate void UnityRefFoundDelegate(string path, string target, Type expectedType);

        public static event UnityRefFoundDelegate OnUnityRefFound;

        private UnityRuntimeObjectConverter() { }

        public override bool CanConvert(Type objectType)
        {
            return typeof(UnityEngine.Object).IsAssignableFrom(objectType);
        }

        private static string Dereference(string path, Type targetType, LoadingContext context) => path;

        public static string TransformRef(string stringRef, LoadingContext context, Type targetType)
        {
            if (stringRef.StartsWith("Assets/"))
                return Dereference(stringRef, targetType, context);

            if (stringRef.StartsWith("./"))
            {
                var combined = Path.Combine("Assets", Path.GetDirectoryName(context.RootAddress).Trim('/', '\\'), stringRef.Substring(2)).Replace('\\', '/');
                return Dereference(combined, targetType, context);
            }

            if (stringRef.StartsWith("/"))
            {
                Logger.IfWarn()?.Message($"Invalid format of UnityRef: {stringRef} at {context.RootAddress}, expecting it to start with 'Assets/' or './'").Write();
                return Dereference("Assets" + stringRef, targetType, context);
            }

            if (stringRef.StartsWith("@"))
            {
                var name = stringRef.Substring(1);
                Type t;
                object var = context.GetVar(name, out t);
                return ((IUnityRef)var).Path;
            }
            return string.Empty;
        }

        public static string ReadImpl(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var context = (LoadingContext)serializer.Context.Context;

            if (reader.TokenType == JsonToken.Null)
            {
                return string.Empty;
            }
            var path = (string)reader.Value;

            var tgt = TransformRef(path, context, objectType);

            var lineInfo = reader as IJsonLineInfo;

            if (string.IsNullOrWhiteSpace(tgt))
            {
                Logger.IfError()?.Message("Reference {0} at ({1}:{2}) is invalid {3}", path, lineInfo.LineNumber, lineInfo.LinePosition, context.RootAddress).Write();
                return tgt;
            }

            var stringRef = new ResourceIDFull(context.ProtoRootAdress, lineInfo.LineNumber, lineInfo.LinePosition, context.IsProto ? context.ProtoIndex : 0).ToString();
            OnUnityRefFound?.Invoke(stringRef, tgt, objectType);

            return tgt;
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

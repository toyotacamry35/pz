using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using Assets.Src.ResourcesSystem.Base;
using Core.Environment.Logging.Extension;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;

namespace ResourcesSystem.Loader
{
    public class DefReferenceConverter : JsonConverter
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly bool _onDemand;
        private readonly DeserializerImpl deserializer;

        public static event Action<string, Type> OnDefReferenceFound;

        public DefReferenceConverter(DeserializerImpl deserializer, bool onDemand)
        {
            _onDemand = onDemand;
            this.deserializer = deserializer;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType.IsGenericType && objectType.GetGenericTypeDefinition() == typeof(ResourceRef<>);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            while (reader.TokenType == JsonToken.Comment)
                reader.Read();
            var context = (LoadingContext)serializer.Context.Context;

            var tokenType = reader.TokenType;
            var readerValue = reader.Value;
            var lineInfo = ((JsonTextReader)reader);

            try
            {
                var defType = objectType.GetGenericArguments()[0];
                if (tokenType == JsonToken.Null)
                {
                    return CreateRef(defType, null);
                }

                if (tokenType == JsonToken.String || tokenType == JsonToken.PropertyName)
                {
                    var stringRef = readerValue as string;
                    if (stringRef.StartsWith("/"))
                    {
                        //this is an absolute reference to a resource
                        //if I'm loading a prototype and this is not an embedded prototype then I'm loading external prototype, I should not do anything here until I encounter it again
                        //next time it won't be a child proto file
                        if (context.IsProtoChildFile && !context.ProtoStack.Peek().Embedded)
                        {
                            var rRef = CreateRef(context.IsProto, defType, stringRef);
                            return rRef;
                        }

                        if (context.IsProto)
                        {

                            context.PushProto(false);
                            var normRef = CreateRef(context.IsProto, defType, stringRef);
                            context.PopProto();
                            return normRef;
                        }

                        return CreateRef(context.IsProto, defType, stringRef);
                    }
                    else if (stringRef.StartsWith("./"))
                    {
                        //this is a relative reference to a resource
                        if (context.IsProtoChildFile && !context.ProtoStack.Peek().Embedded)
                        {
                            var rRef = CreateRef(context.IsProto, defType, stringRef, context.RootAddress);
                            return rRef;
                        }

                        if (context.IsProto)
                        {

                            context.PushProto(false);
                            var normRef = CreateRef(context.IsProto, defType, stringRef, context.RootAddress);
                            context.PopProto();
                            return normRef;
                        }

                        return CreateRef(context.IsProto, defType, stringRef, context.RootAddress);


                    }
                    else if (stringRef.StartsWith("$"))
                    {
                        //this is a local reference to a resource
                        var id = stringRef.Substring(1);
                        IResource res = context.GetInternalRes(id);
                        if (res != null)
                        {
                            return CreateRef(defType, res);
                        }
                        else
                        GameResources.ThrowError<JsonException>($"Reference to internal def not found {stringRef} {lineInfo.LineNumber} {lineInfo.LinePosition} {context.RootAddress}");
                    }
                    else if (stringRef.StartsWith("@"))
                    {
                        //this a reference to a variable
                        var name = stringRef.Substring(1);
                        Type t;
                        object var = context.GetVar(name, out t);
                        if (var == null)
                        {
                        //Logger.IfWarn()?.Message($"Variable has no value {stringRef} {lineInfo.LineNumber} {lineInfo.LinePosition} {context.RootAddress}").Write();
                            return CreateRef(defType, null);
                        }
                        else if (var is IRefBase)
                        {
                            try
                            {
                                return ConvertToProperRef(var, defType);
                            }
                            catch (Exception e)
                            {
	                            if(GameResources.ThrowExceptions)
	                                throw new JsonException($"Error converting ref variable type {stringRef} at {lineInfo.LineNumber} {lineInfo.LinePosition} {context.RootAddress}", e);
	                            else
	                            {
	                                Logger.IfError()?.Message($"Error converting ref variable type {stringRef} at {lineInfo.LineNumber} {lineInfo.LinePosition} {context.RootAddress}").Write();
	                            }
                            }
                        }
                        else
                        {
                            var attr = defType.GetCustomAttribute<CanBeCreatedFromAliasedPrimitiveAttribute>(inherit: true);
                            if (attr != null)
                            {
                                var objectOfDesiredType = PrimitiveTypesConverter.Convert(var, attr.PrimitiveType);
                                var def = defType.GetMethod(attr.MethodName, BindingFlags.Static | BindingFlags.Public).Invoke(null, new[] {objectOfDesiredType});

                                return CreateRef(defType, (IResource) def);
                            }
                        }
                    }
                }
                else if (tokenType == JsonToken.StartObject)
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
            	GameResources.ThrowError<JsonException>($"Reference to definition is not a string or object {lineInfo.LineNumber} {lineInfo.LinePosition} {context.RootAddress} - is {tokenType} {readerValue}");
            }
            catch (Exception e)
            {
                GameResources.ThrowError(e, context.RootAddress, lineInfo.Path, lineInfo.LineNumber, lineInfo.LinePosition);
            }
            return default(object);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            var refTarget = ((IRefBase)value).TargetBase;
            if (refTarget == null)
            {

                writer.WriteNull();
                return;
            }
            var root = refTarget.Address.Root;
            if (refTarget.IsRef && refTarget.LocalId != null)
                writer.WriteValue(refTarget.LocalId);
            else if (root == null)
                JObject.FromObject(refTarget, serializer).WriteTo(writer);
            else
                writer.WriteValue(root);
        }

        private object CreateRef(bool isProto, Type resType, string refToResource)
        {
            OnDefReferenceFound?.Invoke(refToResource, resType);

            var resId = ResourceIDFull.Parse(refToResource);

            if (!_onDemand || isProto)
            {
                var res = deserializer.LoadResource(resId, resType, isProto);
                var retVal = GetRefCreator(resType)(res);
                return retVal;
            }
            else
            {
                if (deserializer.LoadedResources.GetExisting(resId, resType, out var res))
                {
                    var retVal = GetRefCreator(resType)(res);
                    return retVal;
                }
                else
                {
                    // ReSharper disable once PossibleNullReferenceException
                    var retVal = GetRefCreatorLazy(resType)(deserializer, resId);
                    return retVal;
                }
            }
        }

        private object CreateRef(bool isProto, Type resType, string refToResource, string rootRef)
        {
            //stupid normalization, but honestly I don't know how to do it here, because we're not just working with filesystem but with our dictionary
            //TODO: change to proper normalization
            //bug: only supports ./something/something, not ./../../and/so/on

            var path = rootRef.Substring(0, rootRef.LastIndexOf('/'));
            refToResource = path + refToResource.Substring(1);
            return CreateRef(isProto, resType, refToResource);
        }

        internal object ConvertToProperRef(object var, Type defType)
        {
            var varRef = (IRefBase)var;
            try
            {
                return GetConverter(defType)(varRef);
            }
            catch(Exception)
            {
                GameResources.ThrowError($"Can't convert {defType} {varRef.TargetBase.GetType()}");
            }
            return GetRefCreator(defType)(null);
        }

        internal static object CreateRef(Type resType, IResource res)
        {
            return GetRefCreator(resType)(res);
        }

        private delegate object CreateRefLazyDelegate(IResourcesRepository resourcesRepository, in ResourceIDFull id);
        private delegate object CreateRefDelegate(IResource obj);
        private delegate object ConvertDelegate(IRefBase srcRef);

        private static object CreateRefLazyDelegateImpl<T>(IResourcesRepository resourcesRepository, in ResourceIDFull id) where T : class, IResource => new ResourceRef<T>(resourcesRepository, id);
        private static object CreateRefDelegateImpl<T>(IResource obj) where T : class, IResource => new ResourceRef<T>((T)obj);
        private static object ConvertDelegateImpl<T>(IRefBase srcRef) where T : class, IResource => srcRef.To<T>();
        private static CreateRefLazyDelegate CreateRefCreatorLazyExact<T>() where T : class, IResource => CreateRefLazyDelegateImpl<T>;
        private static CreateRefDelegate CreateRefCreatorExact<T>() where T : class, IResource => CreateRefDelegateImpl<T>;
        private static ConvertDelegate ConvertDelegateExact<T>() where T : class, IResource => ConvertDelegateImpl<T>;

        private static CreateRefLazyDelegate CreateRefCreatorLazy(Type resType)
        {
            var method = MethodBase.GetCurrentMethod().DeclaringType.GetMethod(nameof(CreateRefCreatorLazyExact), BindingFlags.Static | BindingFlags.NonPublic);
            var del = (CreateRefLazyDelegate)method.MakeGenericMethod(resType).Invoke(null, Array.Empty<object>());
            return del;
        }

        private static CreateRefDelegate CreateRefCreator(Type resType)
        {
            var method = MethodBase.GetCurrentMethod().DeclaringType.GetMethod(nameof(CreateRefCreatorExact), BindingFlags.Static | BindingFlags.NonPublic);
            var del = (CreateRefDelegate)method.MakeGenericMethod(resType).Invoke(null, Array.Empty<object>());
            return del;
        }

        private static ConvertDelegate CreateConverter(Type resType)
        {
            var method = MethodBase.GetCurrentMethod().DeclaringType.GetMethod(nameof(ConvertDelegateExact), BindingFlags.Static | BindingFlags.NonPublic);
            var del = (ConvertDelegate)method.MakeGenericMethod(resType).Invoke(null, Array.Empty<object>());
            return del;
        }


        private static CreateRefLazyDelegate GetRefCreatorLazy(Type resType) => _refCtorsLazy.GetOrAdd(resType, CreateRefCreatorLazy);
        private static CreateRefDelegate GetRefCreator(Type resType) => _refCtors.GetOrAdd(resType, CreateRefCreator);
        private static ConvertDelegate GetConverter(Type resType) => _converters.GetOrAdd(resType, CreateConverter);

        private static readonly ConcurrentDictionary<Type, CreateRefLazyDelegate> _refCtorsLazy = new ConcurrentDictionary<Type, CreateRefLazyDelegate>();
        private static readonly ConcurrentDictionary<Type, CreateRefDelegate> _refCtors = new ConcurrentDictionary<Type, CreateRefDelegate>();
        private static readonly ConcurrentDictionary<Type, ConvertDelegate> _converters = new ConcurrentDictionary<Type, ConvertDelegate>();
    }

}

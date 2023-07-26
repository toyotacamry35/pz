using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Assets.ColonyShared.SharedCode.ResourcesSystem.Base;
using Assets.Src.ResourcesSystem.Base;
using Core.Environment.Logging.Extension;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using NLog;

namespace ResourcesSystem.Loader
{
    public class DefConverter : JsonConverter
    {
        //private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly LoadedResourcesHolder loadedResources;
        private readonly DeserializerImpl deserializer;

        public DefConverter(LoadedResourcesHolder loadedResources, DeserializerImpl deserializer)
        {
            this.loadedResources = loadedResources;
            this.deserializer = deserializer;
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(IResource).IsAssignableFrom(objectType);
        }

        struct LineInfo : IJsonLineInfo
        {
            public int Row;
            public int Col;
            public int LineNumber => Row;

            public int LinePosition => Col;

            public bool HasLineInfo()
            {
                return true;
            }
        }


        void ClearComments(JsonReader reader)
        {
            while (reader.TokenType == JsonToken.Comment)
                reader.Read();

        }
        //$type
        //$id
        //$vars
        //$overrideVars
        //$proto
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            ClearComments(reader);
            var context = (LoadingContext)serializer.Context.Context;
            var Context = context;
            var lineInfo = ((JsonTextReader)reader);
            if (reader.TokenType != JsonToken.StartObject)
                GameResources.ThrowError($"Expecting StarObject token at {Context.RootAddress}:{lineInfo.LineNumber}:{lineInfo.LinePosition}, possibly ResourceRef<> wrap is forgotten");

            bool hasId = false;
            bool hasProto = false;
            bool hasOverrideVars = false;
            bool hasVars = false;
            var startPath = (string)reader.Path;
            int startDepth = reader.Depth;
            ClearComments(reader);
            //skip "$type" propertyname
            reader.Read();
            ClearComments(reader);
            if ((string)reader.Value != "$type")
            {
                GameResources.ThrowError($"$type is not first at {lineInfo.LineNumber} {lineInfo.LinePosition} {Context.RootAddress} - val is {reader.Value} {reader.TokenType}");
            }

            reader.Read();
            var specificTypeName = (string)reader.Value;
            reader.Read();

            ClearComments(reader);

            specificTypeName = ExpandTypeName(specificTypeName, objectType);


            Type specificType = null;
            try
            {
                specificType = serializer.SerializationBinder.BindToType(string.Empty, specificTypeName);

            }
            catch(Exception e)
            {
                GameResources.ThrowError($"Error during load {Context.RootAddress}:  {e}");
            }
            string id = null;
            if (reader.TokenType == JsonToken.PropertyName && (string)reader.Value == "$id")
            {
                reader.Read();
                id = (string)reader.Value;
                reader.Read();
                if (id != null)
                    hasId = true;
            }
            IResource defInstance = null;
            if (specificType != null)
            {
                if (Context.IsProto && !Context.ProtoStack.Peek().FinishedLoadingDirectProtos && !Context.ProtoStack.Peek().IsCurrentlyParsingVars)
                    defInstance = (IResource)Context.ProtoStack.Peek().ProtoObject;
                else
                    defInstance = (IResource)Activator.CreateInstance(specificType);

            }
            if (specificType != null)
            {

                if (id != null)
                {
                    Context.SetInternalRes(id, defInstance);
                }

                var addr = Context.IsRootObject
                    ? new ResourceIDFull(Context.RootAddress)
                    : new ResourceIDFull(Context.RootAddress, lineInfo.LineNumber, lineInfo.LinePosition);

                if (!Context.IsProto)
                {
                    ((IResource)defInstance).Address = addr;
                    ((IResource)defInstance).OwningRepository = deserializer;
                    loadedResources.RegisterObject(addr, (BaseResource)defInstance, Context);
                }
                else if (Context.ProtoStack.Peek().ProtoObject != defInstance)
                {
                    var addrId = new ResourceIDFull(Context.ProtoRootAdress, lineInfo.LineNumber, lineInfo.LinePosition,
                        Context.ProtoIndex);
                    ((IResource)defInstance).Address = addrId;
                    ((IResource)defInstance).OwningRepository = deserializer;
                    loadedResources.RegisterObject(addrId, (BaseResource)defInstance, Context);
                }
            }
            try
            {
                if (Context.IsProto)
                    Context.ProtoStack.Peek().IsCurrentlyParsingVars = true;
                ClearComments(reader);
                bool isProto = false;
                if (reader.TokenType == JsonToken.PropertyName)
                {
                    if ((string)reader.Value == "$vars")
                    {
                        if (Context.IsProto)
                            Context.ProtoStack.Peek().FinishedLoadingDirectProtos = true;
                        /*bool can = false;
                        if (Context.ProtoStack.Count > 0)
                        {
                            can = Context.ProtoStack.Peek().CanRegisterDefInstance;
                            Context.ProtoStack.Peek().CanRegisterDefInstance = false;
                        }*/

                        hasVars = true;
                        ClearComments(reader);
                        //skip propName
                        reader.Read();
                        //skip startobject
                        reader.Read();
                        while (reader.TokenType != JsonToken.EndObject)
                        {
                            ClearComments(reader);
                            var varName = (string)reader.Value;
                            //skip to content
                            reader.Read();
                            context.PushSubObject();
                            var temVarObj = serializer.Deserialize(reader, typeof(TemplateVariable));
                            //here I'm at the last token of prev object
                            reader.Read();
                            Context.PopSubObject();
                            var temVar = (TemplateVariable)temVarObj;
                            temVar.VariableId = new ResourceIDFull(Context.RootAddress, lineInfo.LineNumber, lineInfo.LinePosition);
                            if (temVar == null)
                            {
                                GameResources.ThrowError<JsonException>($"Error reading template var {varName} at {lineInfo.LineNumber} {lineInfo.LinePosition} {Context.RootAddress}");
                            }

                            Context.SetVar(varName, temVar);
                        }

                        //skip $vars endObj
                        reader.Read();
                        //if (Context.ProtoStack.Count > 0)
                        //    Context.ProtoStack.Peek().CanRegisterDefInstance = can;

                        if (Context.IsProto)
                            Context.ProtoStack.Peek().FinishedLoadingDirectProtos = false;
                    }

                    ClearComments(reader);
                    if ((string)reader.Value == "$overrideVars" || (string)reader.Value == "$proto")
                    {
                        //bool can = false;
                        //if (Context.ProtoStack.Count > 0)
                        //{
                        //    can = Context.ProtoStack.Peek().CanRegisterDefInstance;
                        //    Context.ProtoStack.Peek().CanRegisterDefInstance = false;
                        //}

                        isProto = true;
                        //here I read "$proto" and "$overrideVars"

                        //load from current context (say if a var points to previously set var)
                        Dictionary<string, KeyValuePair<TemplateVariable, IJsonLineInfo>> vars =
                            new Dictionary<string, KeyValuePair<TemplateVariable, IJsonLineInfo>>();
                        if ((string)reader.Value == "$overrideVars")
                        {
                            hasOverrideVars = true;
                            reader.Read(); //skip propName
                            var startOverrideDepthRead = reader.Depth;
                            reader.Read(); //skip start object
                            while (reader.TokenType != JsonToken.EndObject)
                            {
                                ClearComments(reader);
                                var varLineInfo = (JsonTextReader)reader;
                                object value = null;
                                Type varTypeIfHasOne = null;
                                Context.PushSubObject();
                                if (reader.TokenType != JsonToken.PropertyName)
                                    GameResources.ThrowError("Override var does not start with a property name");

                                var varNameValue = (string)reader.Value;
                                reader.Read(); // value token (start obj, string or whatever)
                                if (reader.TokenType == JsonToken.String)
                                {
                                    var refValue = ((string)reader.Value);
                                    if (refValue.StartsWith("@"))
                                    {
                                        value = Context.GetVar(refValue.Substring(1), out varTypeIfHasOne);

                                    }
                                    else if (refValue.StartsWith("/") || refValue.StartsWith("./") || refValue.StartsWith("$"))
                                        value = serializer.Deserialize(reader, typeof(ResourceRef<BaseResource>));
                                    else if (refValue.StartsWith("Assets/"))
                                        value = serializer.Deserialize(reader, typeof(UnityRef<UnityEngine.Object>));
                                    else
                                    {
                                        value = serializer.Deserialize(reader);
                                    }
                                }
                                else
                                {
                                    value = serializer.Deserialize(reader);
                                }

                                //here I'm at the last token of override var
                                reader.Read();
                                Context.PopSubObject();
                                if (Context.IsProto)
                                    Context.ProtoStack.Peek().IsCurrentlyParsingVars = false;
                                vars.Add(varNameValue,
                                    new KeyValuePair<TemplateVariable, IJsonLineInfo>(
                                        new TemplateVariable() { Type = varTypeIfHasOne, Value = value },
                                        new LineInfo() { Row = varLineInfo.LineNumber, Col = varLineInfo.LinePosition }));
                                ClearComments(reader);
                            }

                            var endOverrideDepthRead = reader.Depth;
                            if (startOverrideDepthRead != endOverrideDepthRead)
                                GameResources.ThrowError($"Depth in override is wrong {startDepth} {endOverrideDepthRead}");

                            //Skipping end object
                            reader.Read();

                            Context.PushProto(true, new ResourceIDFull(Context.RootAddress));
                            foreach (var variable in vars)
                            {
                                var value = variable.Value.Key;
                                var setVar = value.Value;
                                var type = value.Value?.GetType();
                                if (type == null)
                                    type = value.Type;
                                if (type != null)
                                    if (typeof(IResource).IsAssignableFrom(type))
                                    {
                                        setVar = DefReferenceConverter.CreateRef(type, (IResource)value.Value);
                                        if (setVar != null)
                                            type = setVar.GetType();
                                    }

                                Context.SetVar(variable.Key,
                                    new TemplateVariable()
                                    {
                                        Value = setVar,
                                        Type = type,
                                        VariableId = new ResourceIDFull(Context.RootAddress, variable.Value.Value.LineNumber,
                                            variable.Value.Value.LinePosition)
                                    });
                            }
                        }
                        else
                            Context.PushProto(true, new ResourceIDFull(Context.RootAddress)); //Here I could've missed something

                        while (reader.TokenType == JsonToken.Comment)
                            reader.Read();
                        while (reader.TokenType != JsonToken.PropertyName)
                            reader.Read();
                        //Skipping $proto
                        if ((string)reader.Value != "$proto")
                            GameResources.ThrowError(
                                $"No $proto at {lineInfo.LineNumber} {lineInfo.LinePosition} {Context.RootAddress} - is {reader.Value} {reader.TokenType}");
                        hasProto = true;
                        reader.Read();
                        Context.PushSubObject();
                        bool setProtoStart = false;
                        if (setProtoStart = !Context.LoadingFrames.Peek().ProtoStart.HasValue)
                            Context.LoadingFrames.Peek().ProtoStart =
                                new ResourceIDFull(Context.RootAddress, lineInfo.LineNumber, lineInfo.LinePosition);
                        Context.ProtoIndex = Context.ProtoIndex + 1;
                        Context.ProtoStack.Peek().ProtoObject = defInstance;
                        var serObj = serializer.Deserialize(reader, (typeof(ResourceRef<>).MakeGenericType(specificType)));
                        if (reader.TokenType == JsonToken.String || reader.TokenType == JsonToken.EndObject)
                            reader.Read();
                        if (reader.TokenType == JsonToken.PropertyName && (string)reader.Value == "$overrideVars")
                            GameResources.ThrowError(
                                $"$proto and $overrideVars are in a wrong order. Should be $overrideVars and then $proto at {lineInfo.LineNumber} {lineInfo.LinePosition} {Context.RootAddress}");
                        var rRef = (IRefBase)serObj;

                        if (setProtoStart)
                            Context.LoadingFrames.Peek().ProtoStart = null;
                        Context.PopSubObject();
                        Context.PopProto();
                        //if (defInstance.GetType() != specificType)
                        //    GameResources.ThrowError(
                        //        $"Prototype and child resources mismatch {defInstance.GetType().Name} {specificTypeName} at {lineInfo.LineNumber} {lineInfo.LinePosition} {Context.RootAddress}");


                        if (Context.IsProto)
                            Context.ProtoStack.Peek().IsCurrentlyParsingVars = false;

                    }
                }

                

                if (Context.IsProto)
                    Context.ProtoStack.Peek().FinishedLoadingDirectProtos = true;

                Context.PushSubObject();
                var contract = (JsonObjectContract)serializer.ContractResolver.ResolveContract(defInstance.GetType());
                var props = contract.Properties;
                //here I'm at a property, which is the first actual property of an object
                ClearComments(reader);


                var requiredProperties = AcquirePropsHashSet();
                foreach (var prop in props)
                {
                    var required = GetPropertyRequired(prop, contract);
                    if (required == Required.Always || required == Required.AllowNull)
                        requiredProperties.Add(prop);
                }

                var propNames = AcquirePropNamessHashSet();
                try
                {
                    while (reader.TokenType != JsonToken.EndObject)
                    {
                        ClearComments(reader);
                        if (((string)reader.Value) == null)
                            GameResources.ThrowError(
                                $"Not Correct Field ({reader.Value.ToString()}) in Jdb file ({reader.Path}). Please fix it!");
                        var propName = (string)reader.Value;
                        var prop = props.GetProperty(propName, StringComparison.InvariantCultureIgnoreCase);
                        if (prop != null)
                        {
                            reader.Read(); //here we are reading value
                            //bool isObj = reader.TokenType == JsonToken.StartObject;
                            var value = serializer.Deserialize(reader, prop.PropertyType);
                            if (propNames.Add(propName))
                                prop.ValueProvider.SetValue(defInstance, value);
                            else
                                GameResources.Logger.IfWarn()?.Message($"Duplicate field {propName} in {startPath} {Context.RootAddress}").Write();
                            //here we should be at the last token of previous deserializer
                            reader.Read();
                            ClearComments(reader);
                            requiredProperties.Remove(prop);
                        }
                        else
                        {
                            if (((string)reader.Value).StartsWith("$"))
                                GameResources.ThrowError($"{reader.Value} is in wrong place {startPath} {Context.RootAddress}");

                            /*
                            if ((string)reader.Value == "Id" && !typeof(ISaveableResource).IsAssignableFrom(specificType))
                            {
                                File.AppendAllLines("c:/Users/User1/Downloads/123.txt", new string[] { $"{Context.RootAddress}:{lineInfo.LineNumber}:{lineInfo.LinePosition}" });
                                //File.AppendAllLines("c:/Users/User1/Downloads/123.txt", new string[] { $"Unknown field '{reader.Value}' at {Context.RootAddress} ({lineInfo.LineNumber}, {lineInfo.LinePosition})" });
                            }*/
#if JDB_EXTRA_WARNINGS
                            GameResources.Logger.IfError()?.Message($"Unknown field '{reader.Value}' at {Context.RootAddress} ({lineInfo.LineNumber}, {lineInfo.LinePosition})").Write();
#endif
                            reader.Skip();
                            reader.Read();
                        }
                    }

                    foreach (var prop in requiredProperties)
                        GameResources.ThrowError($"Required property is missing: {prop.PropertyName}");
                }
                finally
                {
                    ReleasePropNamessHashSet(propNames);
                    ReleasePropsHashSet(requiredProperties);
                }
            }
            catch (ResourceLoadingException e)
            {
                if (GameResources.ThrowExceptions)
                    throw;
                GameResources.Logger.IfError()?.Message($"Error Loading Resource {defInstance.Address.ToString()} in line #{e.LineNo}, position #{e.LinePos}, type of resource {e.Path}: {e.Message}").Write();
            }
            catch (Exception e)
            {
                if (GameResources.ThrowExceptions)
                    throw new ResourceLoadingException(e, defInstance?.Address.ToString(), lineInfo?.Path, lineInfo?.LineNumber ?? 0, lineInfo?.LinePosition ?? 0);
                GameResources.Logger.IfError()?.Message($"Error Loading Resource {defInstance.Address.ToString()} in line #{lineInfo.LineNumber}, position #{lineInfo.LinePosition}, type of resource {lineInfo.Path}: {e.Message}").Write();
            }

            int endDepth = reader.Depth;
            if (startDepth != endDepth)
                GameResources.ThrowError($"Depth is different {specificTypeName} {startPath} {startDepth} {endDepth} hasId {hasId} hasProto {hasProto} hasOverride {hasOverrideVars} hasVars {hasVars}");
            Context.PopSubObject();
            
            if (defInstance is ISaveableResource saveable)
                loadedResources.RegisterSaveable(saveable);

            return defInstance;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            var refTarget = ((IResource)value);
            if (refTarget == null)
            {

                writer.WriteNull();
                return;
            }
            var root = refTarget.Address.Root;
            if (refTarget.LocalId != null && refTarget.IsRef)
                writer.WriteValue(refTarget.LocalId);
            else if (root == null)
                writer.WriteValue(refTarget);
            else
                writer.WriteValue(root);
        }

        private static readonly ConcurrentStack<HashSet<JsonProperty>> _propsHashSetPool = new ConcurrentStack<HashSet<JsonProperty>>(Enumerable.Range(0, 5).Select(x => new HashSet<JsonProperty>()));
        private static readonly ConcurrentStack<HashSet<string>> _propNamesHashSetPool = new ConcurrentStack<HashSet<string>>(Enumerable.Range(0, 5).Select(x => new HashSet<string>()));


        private HashSet<JsonProperty> AcquirePropsHashSet()
        {
            if (_propsHashSetPool.TryPop(out var result))
                return result;
            return new HashSet<JsonProperty>();
        }

        private static void ReleasePropsHashSet(HashSet<JsonProperty> hashset)
        {
            hashset.Clear();
            _propsHashSetPool.Push(hashset);
        }

        private HashSet<string> AcquirePropNamessHashSet()
        {
            if (_propNamesHashSetPool.TryPop(out var result))
                return result;
            return new HashSet<string>();
        }

        private static void ReleasePropNamessHashSet(HashSet<string> hashset)
        {
            hashset.Clear();
            _propNamesHashSetPool.Push(hashset);
        }

        private Required GetPropertyRequired(JsonProperty property, JsonObjectContract contract)
        {
            if (property.Ignored)
                return Required.Default;

            if (property.IsRequiredSpecified)
                return property.Required;
            else
                return contract.ItemRequired.HasValue ? contract.ItemRequired.Value : Required.Default;
        }

        public static string ExpandTypeName(string typeName, Type baseType)
        {
            var containerType = baseType.GetContainerType();
            if (containerType != null && !typeName.StartsWith(containerType.Name))
                typeName = containerType.Name + "." + typeName;
            return typeName;
        }
    }
}
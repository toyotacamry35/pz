using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Core.Reflection;
using SharedCode.Logging;
using ProtoBuf;
using ProtoBuf.Meta;
using SharedCode.EntitySystem;
using SharedCode.Serializers.Protobuf.Surrogates;
using SharedCode.Utils;
using SharedCode.Utils.Threads;
using ColonyShared.SharedCode;
using Core.Environment.Logging.Extension;
using Serializers.MemoryStreams;
using SharedCode.EntitySystem.ChainCalls;

namespace SharedCode.Serializers.Protobuf
{
    public class ProtoBufSerializer : ISerializer
    {
        public T Deserialize<T>(byte[] data) 
        {
            try
            {
                checkInit();

                if (data == null)
                    return default(T);
                
                if (data.Length == 2 && data[0] == 0 && data[1] == 0)
                    return default(T);

                var stream = _reusableMemoryStreamPool.Take();
                try
                {
                    stream.SetBuffer(data, 0, data.Length, false);
                    return (T) _serializer.Deserialize(stream, null, typeof(T));
                }
                finally
                {
                    _reusableMemoryStreamPool.Return(stream);
                }
            }
            catch (Exception e)
            {
                string dumpId = "auto" + typeof(T).GetFriendlyName() + Guid.NewGuid().ToString().Replace("-", "");
                Log.Logger.IfError()?.Message("Error merge {0} bytedata length {1} dumpId {2}, exception {3}", typeof(T).Name,
                    data?.Length.ToString() ?? "null", dumpId, e.ToString());
                TaskEx.Run(() => PathUtils.DumpData(dumpId, data));
                throw;
            }
        }

        public T Deserialize<T>(byte[] data, ref int offset)
        {
            try
            {
                checkInit();

                if (data == null)
                    return default(T);

                if (data.Length >= offset + 2 && data[offset] == 0 && data[offset + 1] == 0)
                {
                    offset += 2;
                    return default(T);
                }

                var stream = _reusableMemoryStreamPool.Take();
                try
                {
                    stream.SetBuffer(data, offset, data.Length - offset, false);
                    var result =
                        (T) _serializer.DeserializeWithLengthPrefix(stream, null, typeof(T), PrefixStyle.Base128, 1);
                    offset += (int) stream.Position;

                    return result;
                }
                finally
                {
                    _reusableMemoryStreamPool.Return(stream);
                }
            }
            catch (Exception e)
            {
                string dumpId = "auto" + typeof(T).GetFriendlyName() + Guid.NewGuid().ToString().Replace("-", "");
                Log.Logger.Error("Error deserializing {0} bytedata length {1} offset {2} dumpId {3} exception {4}",
                    typeof(T).Name, data?.Length.ToString() ?? "null", offset, dumpId, e.ToString());
                TaskEx.Run(() => PathUtils.DumpData(dumpId, data));
                //var datastr = string.Join("-", data);
                //Log.Logger.IfError()?.Message("Error deserializing dataStr {0}", datastr).Write();
                throw;
            }
        }

        public T Merge<T>(T obj, byte[] data) 
        {
            try
            {
                checkInit();

                if (data == null)
                    return default;

                if (data.Length == 2 && data[0] == 0 && data[1] == 0)
                    return default;

                var stream = _reusableMemoryStreamPool.Take();
                try
                {
                    stream.SetBuffer(data, 0, data.Length, false);
                    return (T) _serializer.Deserialize(stream, obj, typeof(T));
                }
                finally
                {
                    _reusableMemoryStreamPool.Return(stream);
                }
            }
            catch (Exception e)
            {
                string dumpId = "auto" + obj.GetType().GetFriendlyName() + Guid.NewGuid().ToString().Replace("-", "");
                Log.Logger.IfError()?.Message("Error merge {0} bytedata length {1} dumpId {2}, exception {3}", obj.GetType().Name,
                    data?.Length.ToString() ?? "null", dumpId, e.ToString());
                TaskEx.Run(() => PathUtils.DumpData(dumpId, data));
                throw;
            }
        }

        public byte[] Serialize<T>(T obj)
        {
            try
            {
                checkInit();
                
                if (obj == null)
                {
                    var buffer = new byte[2];
                    buffer[0] = 0;
                    buffer[1] = 0;
                    return buffer;
                }

                var stream = _memoryStreamPool.Take();
                try
                {
                    _serializer.Serialize(stream, obj);
                    return stream.ToArray();
                }
                finally
                {
                    _memoryStreamPool.Return(stream);
                }
            }
            catch (Exception e)
            {
                Log.Logger.IfError()?.Message(e, "Error Serialize {0} exception", obj?.GetType()?.ToString() ?? "null").Write();
                throw;
            }
        }
        

        public byte[] Serialize(byte[] buffer, ref int offset, object obj, bool full, long replicationMask)
        {
            try
            {
                checkInit();

                if (obj == null)
                {
                    if (buffer.Length < offset + 2)
                    {
                        Log.Logger.Error("Serialize buffer don't have space. Buffer length {0} offset {1}",
                            buffer.Length, offset);
                        throw new InvalidOperationException(string.Format(
                            "Serialize buffer don't have space. Buffer length {0} offset {1}", buffer.Length, offset));
                    }

                    buffer[offset] = 0;
                    buffer[offset + 1] = 0;
                    offset += 2;
                    return buffer;
                }

                var stream = _reusableMemoryStreamPool.Take();
                try
                {
                    stream.SetBuffer(buffer, offset, buffer.Length - offset, true);
                    _serializer.SerializeWithLengthPrefix(stream, obj, obj.GetType(), PrefixStyle.Base128, 1);
                    offset += (int) stream.Position;
                    return buffer;
                }
                finally
                {
                    _reusableMemoryStreamPool.Return(stream);
                }
            }
            catch (NotSupportedException)
            {
                Log.Logger.Error("Serialize buffer don't have space. Buffer length {0} offset {1}", buffer.Length,
                    offset);
                throw new InvalidOperationException(string.Format(
                    "Serialize buffer don't have space. Buffer length {0} offset {1}", buffer.Length, offset));
            }
            catch (Exception e)
            {
                Log.Logger.IfError()?.Message(e, "Error Serialize {0} exception", obj?.GetType()?.ToString() ?? "null").Write();
                throw;
            }
        }

        public byte[] Serialize(byte[] buffer, ref int offset, object obj)
        {
            return Serialize(buffer, ref offset, obj, true, long.MaxValue);
        }

        private static TypeModel _serializer;
        private static object _locker = new object();
        private static bool _inited;
        private static IReadOnlyDictionary<string, Type> _typesContainer;
        private static IReadOnlyDictionary<Type, string> _typeNames;
        private static Pool<MemoryStream> _memoryStreamPool = new Pool<MemoryStream>(500, 50, () => new MemoryStream(),
            x => { x.Position = 0;  x.SetLength(0); });
        private static Pool<ReusableMemoryStream> _reusableMemoryStreamPool =
            new Pool<ReusableMemoryStream>(500, 50, () => new ReusableMemoryStream(), x => x.ReleaseBuffer());

        private static IEnumerable<Assembly> GetCustomAssemblies()
        {
            var allowedAssembliesSubstrs = new List<string>() { "GeneratedCode", "ResourceSystem", "ServerTests", "Core.Cheats" };
            var assemblies = AppDomain.CurrentDomain.GetAssembliesSafe().Where(x => !x.IsDynamic && allowedAssembliesSubstrs.Any(y => x.FullName.Contains(y)));
            return assemblies;
        }

        private static void init()
        {
            try
            {
                var typeModel = TypeModel.Create();
                // typeModel.AutoCompile = false;
                ReusableMemoryStream.Register();
                using (var hasher = new System.Security.Cryptography.SHA1Managed())
                {
                    var typesContainer = new Dictionary<string, Type>();
                    var typeNames = new Dictionary<Type, string>();
                    typeModel.UseImplicitZeroDefaults = false;

                    var assemblies = GetCustomAssemblies().ToList();
                    var allTypesBase = new HashSet<Type>();
                    foreach (var assembly in assemblies)
                    foreach (var type in assembly.GetTypesSafe())
                    {
                        if (!(type.IsAbstract && type.IsSealed)
                            && !typeof(IBaseDeltaObjectWrapper).IsAssignableFrom(type)
                            && !typeof(IChainedEntity).IsAssignableFrom(type)
                            && !type.Name.Contains("_Anonymous")
                            && !type.Name.Contains("<>c")
                            && !type.Name.Contains("<<")
                            && !type.Name.Contains(">d__"))
                            allTypesBase.Add(type);
                    }

                    var closedGenericTypes =
                        new List<Type>(); // Типы из наших сборок, только. Т.е. Dictionary<> там нет
                    {
                        var checkedTypes = new HashSet<Type>();
                        foreach (var type in allTypesBase)
                        {
                            if (!type.CustomAttributes.Any(v => v.AttributeType == typeof(ProtoContractAttribute)))
                                continue;

                            getClosedGenericTypesRecursive(type, closedGenericTypes, checkedTypes, assemblies);
                            //allTypes.AddRange(closedGenericTypes); ??? Там уже есть ВСЕ нужные типы.
                        }
                    }

                    var definitionTypes = allTypesBase.Where(x => x.IsGenericTypeDefinition).ToList();
                    var allTypes = new HashSet<Type>(allTypesBase);
                    foreach (var closedType in closedGenericTypes) // IDeltaDictionary<string, string> MyDict
                    foreach (var definitionType in definitionTypes)
                    {
                        if (definitionType.GetInterfaces().Any(x =>
                                x.IsGenericType && x.GetGenericTypeDefinition() ==
                                closedType.GetGenericTypeDefinition())
                            && definitionType.IsGenericType &&
                            definitionType.GetGenericTypeDefinition().GetGenericArguments().Length ==
                            closedType.GetGenericArguments().Length)
                        {
                            // DeltaDictionary<,>
                            try
                            {
                                var newDefType =
                                    definitionType.MakeGenericType(closedType
                                        .GetGenericArguments()); // DeltaDictionary <string, string>
                                allTypes.Add(newDefType);

                                int typeId = getTypeId(newDefType, hasher);
                                var typeIdStr = typeId.ToString();
                                if (typesContainer.ContainsKey(typeIdStr))
                                {
                                    Log.Logger.Error("ProtoContract typeIds collision id:{0} type1:{1}  type2:{2}",
                                        typeId, typesContainer[typeIdStr].Name, newDefType.Name);
                                    throw new Exception(string.Format(
                                        "ProtoContract typeIds collision id:{0} type1:{1}  type2:{2}", typeId,
                                        typesContainer[typeIdStr].Name, newDefType.Name));
                                }
                                else
                                {
                                    typesContainer.Add(typeIdStr, newDefType);
                                    typeNames.Add(newDefType, typeIdStr);
                                }
                            }
                            catch (Exception e)
                            {
                                Log.Logger.IfError()?.Message(e, "find closedType exception").Write();;
                            }
                        }
                    }

                    foreach (var closedGenericType in closedGenericTypes)
                        allTypes.Add(closedGenericType);
                    generateTypesTree(allTypes);

                    var registeredTypes = new HashSet<Type>();
                    foreach (var type in allTypesBase)
                    {
                        if (registeredTypes.Add(type))
                            RegisterType(typeModel, type, hasher, typesContainer, typeNames);


                        // Костыль для того чтобы работали def'ы унаследованные от generic'ов (например наследники CalcerDef<T>)
                        if (typeof(Assets.Src.ResourcesSystem.Base.IResource).IsAssignableFrom(type))
                            foreach (var bt in GetBaseGenericTypes(type))
                                if (typeof(Assets.Src.ResourcesSystem.Base.IResource).IsAssignableFrom(bt))
                                    if (registeredTypes.Add(bt))
                                        RegisterType(typeModel, bt, hasher, typesContainer, typeNames);
                    }

                    typeModel.DynamicTypeFormatting += Default_DynamicTypeFormatting;
                    _typesContainer = typesContainer;
                    _typeNames = typeNames;
                    
                    typeModel.CompileInPlace();
                    _serializer = typeModel;

                     _inited = true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public static void RegisterType(RuntimeTypeModel typeModel, Type type, System.Security.Cryptography.SHA1Managed hasher, IDictionary<string, Type> typesContainer, IDictionary<Type, string> typeNames)
        {
            if (typeof(Assets.Src.ResourcesSystem.Base.IResource).IsAssignableFrom(type) && !type.ContainsGenericParameters/* && !type.IsAbstract && !type.IsInterface*/)
            {
                try
                {
                    var surrogateType = typeof(ProtobufResourceSurrogate<>).MakeGenericType(type);
                    var t = surrogateType.IsClass;
                    typeModel.Add(surrogateType, true);
                    typeModel.Add(type, false).SetSurrogate(surrogateType);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }

            }
            if(type == typeof(Value))
            {
                typeModel.Add(typeof(ValueSurrogate), true);
                typeModel.Add(type, false).SetSurrogate(typeof(ValueSurrogate));
            }
            if (type.GetCustomAttributes(typeof(ProtoContractAttribute), false).Length > 0)
            {
                int typeId = getTypeId(type, hasher);
                var typeIdStr = typeId.ToString();
                if (typesContainer.ContainsKey(typeIdStr))
                {
                    Log.Logger.IfError()?.Message("ProtoContract typeIds collision id:{0} type1:{1}  type2:{2}", typeId, typesContainer[typeIdStr].Name, type.Name).Write();
                    throw new Exception(string.Format("ProtoContract typeIds collision id:{0} type1:{1}  type2:{2}", typeId, typesContainer[typeIdStr].Name, type.Name));
                }
                else
                {
                    typesContainer.Add(typeIdStr, type);
                    typeNames.Add(type, typeIdStr);
                }

                if (!type.ContainsGenericParameters)
                {
                    typeModel.Add(type, true);
                }
            }

            if (type.GetCustomAttributes(typeof(AutoProtoIncludeSubTypesAttribute), false).Length > 0)
            {
                addSubclassesRecursive(typeModel, type, hasher);
            }
        }
        
        public static IEnumerable<Type> GetBaseGenericTypes(Type type)
        {
            while (type != null)
            {
                if (type.IsGenericType)
                    yield return type;
                type = type.BaseType;
            }
        }

        public static string GetSchema(Type type)
        {
            var typeModel = _serializer.GetSchema(type);
            var result = typeModel?.ToString() ?? "none";
            return result;
        }

        private static void getClosedGenericTypesRecursive(Type rootType, List<Type> closedGenericTypes, HashSet<Type> checkedTypes, List<Assembly> assemblies)
        {
            if (!assemblies.Contains(rootType.Assembly))
                return;

            if (typeof(IBaseDeltaObjectWrapper).IsAssignableFrom(rootType))
                return;

            if (!checkedTypes.Add(rootType))
                return;

            if (rootType.IsGenericType && !rootType.IsGenericTypeDefinition)
            {
                closedGenericTypes.Add(rootType);

                foreach (var arg in rootType.GetGenericArguments())
                    getClosedGenericTypesRecursive(arg, closedGenericTypes, checkedTypes, assemblies);
            }

            foreach (var fieldInfo in rootType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public))
                getClosedGenericTypesRecursive(fieldInfo.FieldType, closedGenericTypes, checkedTypes, assemblies);
        }

        private static void Default_DynamicTypeFormatting(object sender, TypeFormatEventArgs args)
        {
            if (args.Type != null)
            {
                if (!_typeNames.TryGetValue(args.Type, out var typeName))
                    throw new Exception($"_typesContainer {args.Type} is not registered");

                args.FormattedName = typeName;
            }
            else
            {
                if (!_typesContainer.TryGetValue(args.FormattedName, out var type))
                    throw new Exception($"_typesContainer {args.FormattedName} not found");
                args.Type = type;
            }
        }

        static bool IsDirectBaseInterface(Type type, Type interfaceType)
        {
            var interfaces = type.GetInterfaces();
            if (interfaces.All(x => x != interfaceType))
                return false;

            if (type.BaseType != null && IsDirectBaseInterface(type.BaseType, interfaceType))
                return false;

            var containsOtherImplementator = interfaces.Where(x => x != interfaceType).Any(x => IsDirectBaseInterface(x, interfaceType));
            if (containsOtherImplementator)
                return false;

            return true;
        }

        struct TypeTreeNode
        {
            public Type Type;

            public List<Type> InheritedClasses;
        }

        private static Dictionary<Type, TypeTreeNode> _typeTreeNodes = new Dictionary<Type, TypeTreeNode>();

        static void addToTypeTreeNodes(Type baseType, Type inheritedType)
        {
            TypeTreeNode typeTreeNode;
            if (!_typeTreeNodes.TryGetValue(baseType, out typeTreeNode))
            {
                typeTreeNode = new TypeTreeNode
                {
                    Type = baseType,
                    InheritedClasses = new List<Type>()
                };
                _typeTreeNodes[baseType] = typeTreeNode;
            }

            typeTreeNode.InheritedClasses.Add(inheritedType);
        }

        static void generateTypesTree(HashSet<Type> allTypes)
        {
            var checkTypes = allTypes.Where(x => x != typeof(object) &&
                                                    x != typeof(System.ValueTuple) &&
                                                    x != typeof(BaseDeltaObject) && 
                                                    x != typeof(BaseEntity) &&
                                                    x != typeof(Attribute) &&
                                                    !x.IsSubclassOf(typeof(Attribute)) &&
                                                    !typeof(IBaseDeltaObjectWrapper).IsAssignableFrom(x) && 
                                                    !x.IsGenericTypeDefinition).ToList();

            foreach (var inheritedType in checkTypes)
            {
                if (!inheritedType.IsInterface && inheritedType.BaseType != null && inheritedType.BaseType != typeof(object) && inheritedType.BaseType != typeof(System.ValueTuple))
                    addToTypeTreeNodes(inheritedType.BaseType, inheritedType);

                foreach (var baseInterfaceType in inheritedType.GetInterfaces())
                {
                    var needAddSubtype = IsDirectBaseInterface(inheritedType, baseInterfaceType);
                    //if (!needAddSubtype)
                    //    needAddSubtype =
                    //        type.IsGenericTypeDefinition && subtype.IsGenericType && !subtype.IsGenericTypeDefinition &&
                    //        type != subtype
                    //        && subtype.GetGenericTypeDefinition() == type;
                    //if (!needAddSubtype)
                    //{
                    //    needAddSubtype =  baseType.IsGenericType && inheritedType.GetInterfaces().Any(x => x == baseType);
                    //}

                    if (needAddSubtype)
                    {
                        addToTypeTreeNodes(baseInterfaceType, inheritedType);
                    }
                }
            }
        }


        static void addSubclassesRecursive(RuntimeTypeModel typeModel, Type baseType, System.Security.Cryptography.SHA1Managed hasher)
        {
            var metaType = typeModel[baseType];

            if (_typeTreeNodes.TryGetValue(baseType, out var typeTreeNode))
            {
                foreach (var subtype in typeTreeNode.InheritedClasses)
                {
                    int typeId = getTypeId(subtype, hasher);
                    try
                    {
                        metaType.AddSubType(typeId, subtype);

                        if (subtype.IsSubclassOf(typeof(BaseDeltaObject)))
                            copyProtoMemberFields(typeModel, typeof(BaseDeltaObject), subtype);

                        if (subtype.IsSubclassOf(typeof(BaseEntity)))
                            copyProtoMemberFields(typeModel, typeof(BaseEntity), subtype);
                    }
                    catch (Exception e)
                    {
                        Log.Logger.IfError()?.Message("Cant add subtype subTypeId {0} subtype {1} type {2} exception {3}", typeId, subtype.GetFriendlyName(), baseType.GetFriendlyName(), e.ToString()).Write();
                        throw;
                    }

                    addSubclassesRecursive(typeModel, subtype, hasher);
                }
            }
        }

        private static void copyProtoMemberFields(RuntimeTypeModel typeModel, Type fromType, Type toType)
        {
            var fromMetaType = typeModel[fromType];
            var toMetaType = typeModel[toType];
            foreach (var field in fromMetaType.GetFields())
            {
                var valueMember = toMetaType.AddField(field.FieldNumber, field.Name, true);
                valueMember.DataFormat = field.DataFormat;
                valueMember.MapKeyFormat = field.MapKeyFormat;
                valueMember.MapValueFormat = field.MapValueFormat;
                valueMember.AsReference = field.AsReference;
                valueMember.DynamicType = field.DynamicType;
                valueMember.OverwriteList = field.OverwriteList;
                valueMember.SupportNull = field.SupportNull;
                valueMember.DefaultValue = field.DefaultValue;
                valueMember.IsMap = field.IsMap;
                valueMember.IsPacked = field.IsMap;
                valueMember.IsRequired = field.IsRequired;
                valueMember.IsStrict = field.IsStrict;
                valueMember.BackingMember = field.BackingMember;
                valueMember.Name = field.Name;

                var getMethod = fromType.GetMethod("ShouldSerialize" + field.Name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                var specified = fromType.GetProperty(field.Name + "Specified", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (specified != null && specified.GetMethod != null)
                    getMethod = specified.GetMethod;
                var setMethod = specified?.SetMethod;
                if (getMethod != null || setMethod != null)
                    valueMember.SetSpecified(getMethod, setMethod);
            }
        }

        static int getTypeId(Type type, System.Security.Cryptography.SHA1Managed hasher)
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes(type.GetFriendlyName());
            var sha = hasher.ComputeHash(bytes);
            var hash = Math.Abs(BitConverter.ToInt32(sha, 0));
            var res = 0xffffff & Math.Abs(hash + 256);
            return res;
        }

        public static void TryInit(Assembly[] assemblies)
        {
            checkInit();
        }

        private static void checkInit()
        {
            if (!_inited)
            {
                lock (_locker)
                {
                    if (!_inited)
                    {
                        var sw = new Stopwatch();
                        sw.Start();
                        init();
                        sw.Stop();
                        Log.Logger.IfInfo()?.Message("Protobuf initialization time: {0} seconds", sw.Elapsed.TotalSeconds).Write();
                    }
                }
            }
        }
    }
    
    public readonly struct DeserializedObjectInfo
    {
        public DeserializedObjectInfo(IDeltaObject deltaObject, Dictionary<int, ulong?> changedFields)
        {
            DeltaObject = deltaObject;
            ChangedFields = changedFields;
        }

        public IDeltaObject DeltaObject { get; }
        public Dictionary<int, ulong?> ChangedFields { get; }
    }
}

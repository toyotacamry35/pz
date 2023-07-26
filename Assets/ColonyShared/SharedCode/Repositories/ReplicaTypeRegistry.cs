using NLog;
using SharedCode.Cloud;
using SharedCode.EntitySystem;
using SharedCode.Network;
using Core.Reflection;
using SharedCode.Refs;
using SharedCode.Serializers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#if UNITY_5_3_OR_NEWER
using UnityEngine;
#endif

namespace SharedCode.Repositories
{
    public static class ReplicaTypeRegistry
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();
        public delegate void IncomingMessageFunc(IEntitiesRepository repository, INetworkProxy networkProxy, Guid callback, byte[] data, int offset, PropertyAddress address, IDeltaObject obj, Guid transactionId, Guid migrationId);
        public delegate Task ExecuteFunc(IDeltaObject __deltaObj__, byte[] __data__, SharedCode.Entities.Core.IChainContext chainContext, string saveResultKey, Dictionary<int, string> argumentRefs);

        private delegate IEntity ConstructDelegate(Guid id);
        private delegate IEntity DeserializeDelegate(IEntitySerializer serializer, IEntitiesRepository repo, int rootEntityTypeId,
            Guid rootEntityId, Dictionary<ulong, byte[]> snapshot);
        private delegate IEntityRef SetDelegate(IEntity entity, long replicationMask, int version, ConcurrentDictionary<Guid, IEntityRef> collection);

        private static readonly Dictionary<Type, int> _typeToId = new Dictionary<Type, int>();
        private static readonly Dictionary<string, Type> _typeNameToType = new Dictionary<string, Type>();
        private static readonly Dictionary<int, Type> _idToType = new Dictionary<int, Type>();
        private static readonly Dictionary<Type, ValueTuple<DatabaseSaveType, DatabaseServiceType>> _interfaceTypeToDatabaseSaveType = new Dictionary<Type, ValueTuple<DatabaseSaveType, DatabaseServiceType>>();
        private static readonly Dictionary<Type, Type> _replicaTypeToMasterType = new Dictionary<Type, Type>();
        private static readonly Dictionary<Type, ReplicationLevel> _replicaTypeToReplicationLevel = new Dictionary<Type, ReplicationLevel>();
        private static readonly Dictionary<(Type masterType, ReplicationLevel replicationLevel), Type> _masterTypeAndReplicationLevelToReplicaType = new Dictionary<(Type masterType, ReplicationLevel replicationLevel), Type>();

        private static readonly Dictionary<CloudNodeType, List<Type>> _cloudNodeTypeToDefaultServiceEntities = new Dictionary<CloudNodeType, List<Type>>();
        private static readonly Dictionary<Type, Func<CloudNodeType, bool>> _entityTypeNeedReplicateToNodeType = new Dictionary<Type, Func<CloudNodeType, bool>>();

        private static readonly Dictionary<Type, ConstructDelegate> _interfaceTypesToConstructors = new Dictionary<Type, ConstructDelegate>();
        private static readonly Dictionary<Type, DeserializeDelegate> _interfaceTypesToDeserializers = new Dictionary<Type, DeserializeDelegate>();
        private static readonly Dictionary<Type, SetDelegate> _interfaceTypesToSetters = new Dictionary<Type, SetDelegate>();

        private static readonly Dictionary<(Type implClass, byte methodIndex), (IncomingMessageFunc handler, string displayName)> _incomingMessages = new Dictionary<(Type implClass, byte methodIndex), (IncomingMessageFunc handler, string displayName)>();
        private static readonly Dictionary<(Type implClass, byte methodIndex), (ExecuteFunc handler, string displayName)> _executeMethods = new Dictionary<(Type implClass, byte methodIndex), (ExecuteFunc handler, string displayName)>();

        public static int NetworkHash { get; private set; } = 0;

        static ReplicaTypeRegistry()
        {
            _cloudNodeTypeToDefaultServiceEntities[CloudNodeType.Client] = new List<Type>();
            _cloudNodeTypeToDefaultServiceEntities[CloudNodeType.Server] = new List<Type>();

            var allTypes = AppDomain.CurrentDomain.GetAssembliesSafeNonStandard().SelectMany(x => x.GetTypesSafe()).ToArray();

            IEnumerable<(Type implType, Type ifaceType, int typeId)> replicaClasses =
                from type in allTypes
                where !type.IsAbstract && type.IsClass
                let attr = type.CustomAttributes.FirstOrDefault(x => x.AttributeType == typeof(ReplicationClassImplementationAttribute))
                where attr != null
                select (type, (Type)attr.ConstructorArguments[0].Value, (int)attr.ConstructorArguments[1].Value);

            foreach (var type in replicaClasses)
            {
#if UNITY_5_3_OR_NEWER
                try
                {
#endif                    
                    RegisterImplClass(type.typeId, type.implType, type.ifaceType);
#if UNITY_5_3_OR_NEWER
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                    throw;
                }
#endif                    
            }

            IEnumerable<(int typeId, Type type, Type masterType, ReplicationLevel replicationLevel)> replicaInterfaces = 
                from type in allTypes
                where type.IsInterface
                let attr = type.CustomAttributes.FirstOrDefault(x => x.AttributeType == typeof(ReplicationInterfaceAttribute))
                where attr != null
                select ((int)attr.ConstructorArguments[1].Value, type, (Type)attr.ConstructorArguments[2].Value, (ReplicationLevel)attr.ConstructorArguments[0].Value);

            foreach (var type in replicaInterfaces)
                RegisterType(type.typeId, type.type, type.masterType, type.replicationLevel);

            _replicaTypeToMasterType.Add(typeof(IDeltaObject), typeof(IDeltaObject));
            _replicaTypeToReplicationLevel.Add(typeof(IDeltaObject), ReplicationLevel.Master);
        }

        private static void RegisterType(int typeId, Type type, Type masterType, ReplicationLevel replicationLevel)
        {
            _typeToId.Add(type, typeId);
            _idToType.Add(typeId, type);

            _replicaTypeToMasterType.Add(type, masterType);
            _replicaTypeToReplicationLevel.Add(type, replicationLevel);
            _masterTypeAndReplicationLevelToReplicaType.Add((masterType, replicationLevel), type);
        }

        private static void RegisterImplClass(int typeId, Type type, Type interfaceType)
        {
            _typeToId.Add(interfaceType, typeId);
            _idToType.Add(typeId, interfaceType);

            _typeNameToType.Add(interfaceType.Name, interfaceType);

            _replicaTypeToMasterType.Add(interfaceType, interfaceType);
            _replicaTypeToReplicationLevel.Add(interfaceType, ReplicationLevel.Master);
            _masterTypeAndReplicationLevelToReplicaType.Add((interfaceType, ReplicationLevel.Master), interfaceType);
            GatherRpcReceivers(type, interfaceType);
            GatherExecuteReceivers(type, interfaceType);

            if (typeof(IEntity).IsAssignableFrom(interfaceType))
            {
                var attribute = interfaceType.GetCustomAttributes(typeof(EntityServiceAttribute), false).Cast<EntityServiceAttribute>().SingleOrDefault();
                if (attribute != null)
                {
                    if (attribute.AddedByDefaultTo(CloudNodeType.Client))
                        _cloudNodeTypeToDefaultServiceEntities[CloudNodeType.Client].Add(interfaceType);
                    if (attribute.AddedByDefaultTo(CloudNodeType.Server))
                        _cloudNodeTypeToDefaultServiceEntities[CloudNodeType.Server].Add(interfaceType);
                    _entityTypeNeedReplicateToNodeType.Add(interfaceType, (cnt) => attribute.ReplicateTo(cnt));
                }

                var attr = interfaceType.CustomAttributes.SingleOrDefault(x => x.AttributeType == typeof(DatabaseSaveTypeAttribute));
                if (attr != null)
                {
                    var serviceType = DatabaseServiceType.GameData;
                    if (attr.ConstructorArguments.Count > 1)
                        serviceType = (DatabaseServiceType) attr.ConstructorArguments[1].Value;
                    _interfaceTypeToDatabaseSaveType.Add(interfaceType, ((DatabaseSaveType)attr.ConstructorArguments[0].Value, serviceType));
                }
                else
                    _interfaceTypeToDatabaseSaveType.Add(interfaceType, (DatabaseSaveType.None, DatabaseServiceType.None));
                
                _interfaceTypesToConstructors.Add(interfaceType, CreateConstructor(type));
                _interfaceTypesToDeserializers.Add(interfaceType,
                    (serializer, repo, rootEntityTypeId, rootEntityId, snapshot)
                        => serializer.DeserializeEntity(repo, rootEntityTypeId, rootEntityId, snapshot));
                _interfaceTypesToSetters.Add(interfaceType, CreateSetter(interfaceType));
            }
        }

        private static void GatherRpcReceivers(Type implType, Type interfaceType)
        {
            var recvFns = implType.GetNestedType("ReceiveFuncs", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
            if (recvFns == null)
                return;
            int hash = (int)recvFns.CustomAttributes.Single(v => v.AttributeType == typeof(RpcClassHashAttribute)).ConstructorArguments[0].Value;

            NetworkHash ^= hash;

            var methods = recvFns.GetMethods(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
            var rpcMethodsQ = from method in methods
                              let attr = method.CustomAttributes.SingleOrDefault(x => x.AttributeType == typeof(RpcMethodReceiverFuncAttribute))
                              where attr != null
                              let idx = (byte)attr.ConstructorArguments[0].Value
                              let name = (string)attr.ConstructorArguments[1].Value
                              let handler = (IncomingMessageFunc)method.CreateDelegate(typeof(IncomingMessageFunc))
                              select (handler, idx, name);

            foreach (var method in rpcMethodsQ)
            {
                var key = (interfaceType, method.idx);
                if (!_incomingMessages.ContainsKey(key))
                    _incomingMessages.Add(key, (method.handler, method.name));
                else
                    throw new Exception($"RPC receiver conflict. Try to rebuild generated code. Key:{key} Conflicting methods:[{_incomingMessages[key].handler.Method.Name}, {method.handler.Method.Name}]");
            }
        }

        private static void GatherExecuteReceivers(Type implType, Type interfaceType)
        {
            var recvFns = implType.GetNestedType("ExecuteMethods", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
            if (recvFns == null)
                return;

            var methods = recvFns.GetMethods(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
            var rpcMethodsQ = from method in methods
                              let attr = method.CustomAttributes.SingleOrDefault(x => x.AttributeType == typeof(RpcMethodExecuteFuncAttribute))
                              where attr != null
                              let idx = (byte)attr.ConstructorArguments[0].Value
                              let name = (string)attr.ConstructorArguments[1].Value
                              let handler = (ExecuteFunc)method.CreateDelegate(typeof(ExecuteFunc))
                              select (handler, idx, name);

            foreach (var method in rpcMethodsQ)
            {
                var key = (interfaceType, method.idx);
                if (!_executeMethods.ContainsKey(key))
                    _executeMethods.Add(key, (method.handler, method.name));
                else
                    throw new Exception($"Execute receiver conflict. Try to rebuild generated code. Key:{key} Conflicting methods:[{_executeMethods[key].handler.Method.Name}, {method.handler.Method.Name}]");
            }
        }

        private static ConstructDelegate CreateConstructor(Type type)
        {
            var constructMethod = type.GetMethod("Construct", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
            return (ConstructDelegate)constructMethod.CreateDelegate(typeof(ConstructDelegate));
        }

        private static SetDelegate CreateSetter(Type type)
        {
            var unbound = typeof(Adder<>);
            var bound = unbound.MakeGenericType(type);
            var method = bound.GetMethod(nameof(Adder<IEntity>.AddToCollection), System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
            return (SetDelegate)method.CreateDelegate(typeof(SetDelegate));
        }

        private static class Adder<T> where T : IEntity
        {
            private static readonly SetDelegate TestCompatibility = AddToCollection;
            public static IEntityRef AddToCollection(IEntity entity, long replicationMask, int version, ConcurrentDictionary<Guid, IEntityRef> collection)
            {
                var newEntityRef = new EntityRef<T>((T)entity, replicationMask, version);
                collection[newEntityRef.Id] = newEntityRef;
                return newEntityRef;
            }
        }

        public static Type GetTypeById(int id)
        {
            if(_idToType.TryGetValue(id, out var type))
                return type;
            throw new KeyNotFoundException($"Id {id} is unknown to entity system");
        }
        public static int GetIdByType(Type type)
        {
            if (_typeToId.TryGetValue(type, out var id))
                return id;
            throw new KeyNotFoundException($"Type {type} is unknown to entity system");
        }
        public static Type GetTypeByName(string typeName)
        {
            if(_typeNameToType.TryGetValue(typeName, out var type))
                return type;
            throw new KeyNotFoundException($"Type {typeName} is unknown to entity system");
        }

        public static DatabaseSaveType GetDatabaseSaveType(Type type)
        {
            if (_interfaceTypeToDatabaseSaveType.TryGetValue(type, out var pair))
                return pair.Item1;
            throw new KeyNotFoundException($"Type {type} is unknown to entity system");
        }
        public static DatabaseServiceType GetDatabaseServiceType(Type type)
        {
            if (_interfaceTypeToDatabaseSaveType.TryGetValue(type, out var pair))
                return pair.Item2;
            throw new KeyNotFoundException($"Type {type} is unknown to entity system");
        }

        public static Type GetMasterTypeByReplicationLevelType(Type type)
        {
            if (_replicaTypeToMasterType.TryGetValue(type, out var masterType))
                return masterType;
            throw new KeyNotFoundException($"Type {type} is unknown to entity system");
        }
        public static ReplicationLevel GetReplicationLevelByReplicaType(Type replicationType)
        {
            if (_replicaTypeToReplicationLevel.TryGetValue(replicationType, out var level))
                return level;
            throw new KeyNotFoundException($"Type {replicationType} is unknown to entity system");
        }
        public static Type GetReplicationTypeId(Type masterType, ReplicationLevel replicationLevel)
        {
            if (_masterTypeAndReplicationLevelToReplicaType.TryGetValue((masterType, replicationLevel), out var type))
                return type;
            throw new KeyNotFoundException($"Cant find level {replicationLevel} for type {masterType}");
        }

        public static IReadOnlyList<Type> GetServiceEntitiesForNodeType(CloudNodeType node) => _cloudNodeTypeToDefaultServiceEntities[node];
        public static Func<CloudNodeType, bool> NeedReplicateEntityTypeTo(Type interfaceType)
        {
            if (_entityTypeNeedReplicateToNodeType.TryGetValue(interfaceType, out var fn))
                return fn;
            throw new KeyNotFoundException($"Type {interfaceType} is not registered as service entity");
        }

        public static IEntity CreateImpl(Type interfaceType, Guid id)
        {
            if(_interfaceTypesToConstructors.TryGetValue(interfaceType, out var ctor))
                return ctor(id);
            throw new KeyNotFoundException($"Constructor not found for {interfaceType}");
        }

        public static IEntity Deserialize(Type interfaceType, IEntitySerializer serializer, IEntitiesRepository repo,
            int rootEntityTypeId, Guid rootEntityId, Dictionary<ulong, byte[]> snapshot)
        {
            if (_interfaceTypesToDeserializers.TryGetValue(interfaceType, out var deserializer))
                return deserializer(serializer, repo, rootEntityTypeId, rootEntityId, snapshot);
            throw new KeyNotFoundException($"Deserializer not found for {interfaceType}");
        }

        public static IEntityRef Set(Type interfaceType, IEntity entity, long replicationMask, int version, ConcurrentDictionary<Guid, IEntityRef> collection)
        {
            if(_interfaceTypesToSetters.TryGetValue(interfaceType, out var setter))
                return setter(entity, replicationMask, version, collection);
            throw new KeyNotFoundException($"Setter not found for {interfaceType}");
        }

        public static (IncomingMessageFunc dispatcher, string messageName) GetDispatcher(Type interfaceType, byte idx)
        {
            if (_incomingMessages.TryGetValue((interfaceType, idx), out var dispatcher))
                return dispatcher;
            throw new KeyNotFoundException($"Dispatcher not found for {interfaceType}, method index {idx}");
        }

        public static (ExecuteFunc dispatcher, string messageName) GetExecuteFunc(Type interfaceType, byte idx)
        {
            if (_executeMethods.TryGetValue((interfaceType, idx), out var dispatcher))
                return dispatcher;
            throw new KeyNotFoundException($"Execute func not found for {interfaceType}, method index {idx}");
        }
    }
}

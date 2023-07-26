using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProtoBuf;
using ResourcesSystem.Base;
using SharedCode.Refs;
using SharedCode.Serializers.Protobuf;

namespace SharedCode.EntitySystem
{
    public delegate Task PropertyChangedDelegate(EntityEventArgs args);

    [ProtoContract]
    [AutoProtoIncludeSubTypes]
    public interface IDeltaObject: IHasRandomFill, ICanRandomFill
    {
        int TypeId { get; }

        string TypeName { get; }

        int ParentTypeId { get; }

        Guid ParentEntityId { get; }

        IEntitiesRepository EntitiesRepository { get; }

        Guid  OwnerRepositoryId { get; }

        void ClearDelta();

        void GetAllLinkedEntities(long replicationMask,
            System.Collections.Generic.List<(long level, SharedCode.Refs.IEntityRef entityRef)> entities,
            long currentLevel,
            bool onlyDbEntities);

        void SubscribePropertyChanged(string propertyName, PropertyChangedDelegate callback);

        void UnsubscribePropertyChanged(string propertyName, PropertyChangedDelegate callback);

        void UnsubscribePropertyChanged(string propertyName);

        bool NeedFireEvents();

        void UnsubscribeAll();

        bool IsChanged();

        void ProcessEvents(List<Func<Task>> container);

        bool TryGetProperty<T>(int address, out T property);

        IDeltaObject GetReplicationLevel(ReplicationLevel replicationLevel);
    }

    public interface IDeltaObjectExt
    {
        IEntity GetParentEntity();

        void SetParentDeltaObject(IDeltaObjectExt parentDeltaObject);

        void FillReplicationSetRecursive(Dictionary<ReplicationLevel, Dictionary<IDeltaObject, DeltaObjectReplicationInfo>> replicationSets, HashSet<ReplicationLevel> traverseReplicationLevels, ReplicationLevel currentLevel, bool withBsonIgnore);

        void LinkChangedDeltaObjects(Dictionary<ulong, DeserializedObjectInfo> deserializedObjects, IEntity parentEntity);
        
        IDeltaObjectExt GetParentObject();

        void Downgrade(long mask);

        bool ContainsReplicationLevel(long mask);

        void Visit(Action<IDeltaObject> visitor);

        void MarkAllChanged();

        void LinkEntityRefs(IEntitiesRepository repository);
        void SetDirtyReplicationMask(long val);

        bool IsReplicationMaskDirty(long checkedReplicationMask);

        ulong LocalId { get; set; }
        IEntity parentEntity { get; set; }
        IDeltaObjectExt parentDeltaObject { get; set; }
        long LastParentEntityReplicationMask { get; set; }
        short ParentEntityRefCount { get; set; }
        
        void DecrementParentRefs();
        void IncrementParentRefs(IEntity parentEntity, bool trackChanged);
        bool HasParentRef();

        void ReplicationLevelActualize(ReplicationLevel? actualParentLevel, ReplicationLevel? oldParentLevel);

        ref Guid MigratingId { get; }
    }
}

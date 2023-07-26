using SharedCode.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GeneratedCode.EntitySystem;
using GeneratedCode.Repositories;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using ProtoBuf;
using SharedCode.Entities.Core;
using SharedCode.EntitySystem.Delta;
using SharedCode.EntitySystem.EntityPropertyResolvers;
using SharedCode.Refs;

namespace SharedCode.EntitySystem
{
    public abstract partial class BaseEntity
    {
        private object _OwnerNodeId; //боксинг/анбоксинг быстрее lock. А брать стандартным механизмом через репозиторий для чтения этого поля - очень медленно
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public global::System.Guid OwnerNodeId
        {
            get => _OwnerNodeId != null ? (Guid)_OwnerNodeId : Guid.Empty;
            set => GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjField(parentEntity, this, ref _OwnerNodeId, value, SharedCode.EntitySystem.ReplicationLevel.Always, 4, false, false);
        }

        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        [ProtoMember(4)]
        [Newtonsoft.Json.JsonIgnore]
        public global::System.Guid OwnerNodeId__Serialized
        {
            get => _OwnerNodeId != null ? (Guid)_OwnerNodeId : Guid.Empty;
            set => DeltaObjectHelper.SetDeltaObjFieldFromSerialization(parentEntity, this, ref _OwnerNodeId, value, SharedCode.EntitySystem.ReplicationLevel.Always, 4, false);
        }

        event PropertyChangedDelegate OwnerNodeId__Changed;
        public string OwnerNodeId__ChangedSubscribers => global::GeneratedCode.EntitySystem.EventHelper.SubscribersToString(OwnerNodeId__Changed);
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool OwnerNodeId__SerializedSpecified
        {
            get => IsRequiredReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.Always) && IsDirty(4);
            set
            {
                _needUpdateOwnerNode = true;
            }
        }

        [ProtoMember(5, OverwriteList = true)]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public ConcurrentDictionary<Guid, ReplicateRefsContainer> ReplicateRepositoryIds
        {
            get { return _replicateRepositoryIds; }
            set
            {
                _replicateRepositoryIds = value;
                DeltaObjectHelper.SetDeltaObjFieldFromSerialization(parentEntity, this, ref _replicateRepositoryIds, value, SharedCode.EntitySystem.ReplicationLevel.Master, 5, false);
            }
        }

        public bool ShouldSerializeReplicateRepositoryIds() { return IsRequiredReplicationLevel(ReplicationLevel.Master) && IsDirty(5); }

        private global::SharedCode.EntitySystem.Delta.IDeltaDictionary<global::System.Guid, global::SharedCode.Entities.Core.IEntityMethodsCallsChain> _ChainCalls;
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public global::SharedCode.EntitySystem.Delta.IDeltaDictionary<global::System.Guid, global::SharedCode.Entities.Core.IEntityMethodsCallsChain> ChainCalls
        {
            get => GeneratedCode.EntitySystem.DeltaObjectHelper.GetDeltaObjField(parentEntity, ref _ChainCalls, false, false);
            set => GeneratedCode.EntitySystem.DeltaObjectHelper.SetDeltaObjField(parentEntity, this, ref _ChainCalls, value, SharedCode.EntitySystem.ReplicationLevel.Master, 6, false, false);
        }

        [ProtoMember(6, AsReference = true, DynamicType = true)]
        [Newtonsoft.Json.JsonIgnore]
        public ulong? ChainCalls__Serialized
        {
            get
            {
                return ((SharedCode.EntitySystem.IDeltaObjectExt)_ChainCalls)?.LocalId;
            }

            set
            {
                GeneratedCode.EntitySystem.DeltaObjectHelper.SetSerializedValue(this, 6, value);
            }
        }

        event PropertyChangedDelegate ChainCalls__Changed;
        public string ChainCalls__ChangedSubscribers => global::GeneratedCode.EntitySystem.EventHelper.SubscribersToString(ChainCalls__Changed);
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool ChainCalls__SerializedSpecified
        {
            get => IsRequiredReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.Master) &&
                   (IsDirty(6) || (_ChainCalls != null && ((IDeltaObject) _ChainCalls).IsChanged()));
            set { }
        }


        private ulong _NextFreeLocalId = 1;// 0 = Detached, 1 = Root Entity
        [ProtoMember(7)]
        public ulong NextFreeLocalId
        {
            get => _NextFreeLocalId;
            set => DeltaObjectHelper.SetDeltaObjFieldFromSerialization(parentEntity, this, ref _NextFreeLocalId, value, SharedCode.EntitySystem.ReplicationLevel.Always, 7, false);
        } 

        public BaseEntity()
        {
            if (SharedCode.Serializers._SerializerContext.Pool.Current.Deserialization)
                LocalId = 1;

            parentEntity = this;
            ((SharedCode.EntitySystem.IEntityExt)this).SetEntitiesRepository(SharedCode.Serializers._SerializerContext.Pool.Current.EntitiesRepository);
            if (!SharedCode.Serializers._SerializerContext.Pool.Current.Deserialization)
            {
                OwnerNodeId = default(global::System.Guid);
                ChainCalls = new DeltaDictionary<global::System.Guid, global::SharedCode.Entities.Core.IEntityMethodsCallsChain>();
            }
        }

        public BaseEntity(Guid id)
        {
            if (SharedCode.Serializers._SerializerContext.Pool.Current.Deserialization)
                LocalId = 1;

            parentEntity = this;
            Id = id;
            ((SharedCode.EntitySystem.IEntityExt)this).SetEntitiesRepository(SharedCode.Serializers._SerializerContext.Pool.Current.EntitiesRepository);
            if (!SharedCode.Serializers._SerializerContext.Pool.Current.Deserialization)
            {
                OwnerNodeId = default(global::System.Guid);
                ChainCalls = new DeltaDictionary<global::System.Guid, global::SharedCode.Entities.Core.IEntityMethodsCallsChain>();
            }
        }

        public override void ClearDelta()
        {
            if (ReplicationChangedMask == 0)
                return;
            base.ClearDelta();
        }
        
        public void ClearChangedObjects()
        {
            ChangedObjects?.Clear();
        }

        public override void GetAllLinkedEntities(long replicationMask,
            System.Collections.Generic.List<(long level, SharedCode.Refs.IEntityRef entityRef)> entities,
            long currentLevel,
            bool onlyDbEntities)
        {
            if ((replicationMask & (long) SharedCode.EntitySystem.ReplicationLevel.Master) == (long) SharedCode.EntitySystem.ReplicationLevel.Master)
                if (_ChainCalls != null)
                    ((SharedCode.EntitySystem.IDeltaObject) _ChainCalls).GetAllLinkedEntities(replicationMask, entities,
                        currentLevel | (long)SharedCode.EntitySystem.ReplicationLevel.Master, onlyDbEntities);

            base.GetAllLinkedEntities(replicationMask, entities, currentLevel, onlyDbEntities);
        }
        
        public override void FillReplicationSetRecursive(Dictionary<ReplicationLevel, Dictionary<IDeltaObject, DeltaObjectReplicationInfo>> replicationSets, HashSet<ReplicationLevel> traverseReplicationLevels, SharedCode.EntitySystem.ReplicationLevel currentLevel, bool withBsonIgnore)
        {
            base.FillReplicationSetRecursive(replicationSets, traverseReplicationLevels, currentLevel, withBsonIgnore);
            DeltaObjectHelper.FillReplicationSet(replicationSets, traverseReplicationLevels,this, currentLevel);
            DeltaObjectHelper.FillReplicationSetRecursive(replicationSets,
                traverseReplicationLevels,
                _ChainCalls,
                currentLevel > ReplicationLevel.Master
                    ? currentLevel
                    : ReplicationLevel.Master,
                true,
                withBsonIgnore);
        }
        

        public override void LinkChangedDeltaObjects(System.Collections.Generic.Dictionary<ulong, SharedCode.Serializers.Protobuf.DeserializedObjectInfo> deserializedObjects, SharedCode.EntitySystem.IEntity parentEntity)
        {
            base.LinkChangedDeltaObjects(deserializedObjects, parentEntity);
            GeneratedCode.EntitySystem.DeltaObjectHelper.LinkChangedDeltaObject(deserializedObjects, parentEntity, this, ref _ChainCalls, 6, false, SharedCode.EntitySystem.ReplicationLevel.Master);
        }

        public override void IncrementParentRefs(SharedCode.EntitySystem.IEntity parentEntity, bool trackChanged)
        {
            base.IncrementParentRefs(parentEntity, trackChanged);
            if (ParentEntityRefCount == 1)
            {
                ((SharedCode.EntitySystem.IDeltaObjectExt)_ChainCalls)?.IncrementParentRefs(parentEntity, trackChanged);
            }
        }

        public override void DecrementParentRefs()
        {
            base.DecrementParentRefs();
            if (!HasParentRef())
            {
                ((SharedCode.EntitySystem.IDeltaObjectExt)_ChainCalls)?.DecrementParentRefs();
            }
        }

        protected override void Subscribe(string propertyName, PropertyChangedDelegate callback)
        {
            base.Subscribe(propertyName, callback);
            switch (propertyName)
            {
                case "OwnerNodeId":
                    OwnerNodeId__Changed += callback;
                    break;
                case "ChainCalls":
                    ChainCalls__Changed += callback;
                    break;
            }
        }

        protected override void Unsubscribe(string propertyName, PropertyChangedDelegate callback)
        {
            base.Unsubscribe(propertyName, callback);
            switch (propertyName)
            {
                case "OwnerNodeId":
                    OwnerNodeId__Changed -= callback;
                    break;
                case "ChainCalls":
                    ChainCalls__Changed -= callback;
                    break;
            }
        }

        protected override void Unsubscribe(string propertyName)
        {
            base.Unsubscribe(propertyName);
            switch (propertyName)
            {
                case "OwnerNodeId":
                    OwnerNodeId__Changed = null;
                    break;
                case "ChainCalls":
                    ChainCalls__Changed = null;
                    break;
            }
        }

        protected override void Unsubscribe()
        {
            base.Unsubscribe();
            OwnerNodeId__Changed = null;
            ChainCalls__Changed = null;
        }

        protected override void FireEvents(List<Func<Task>> container)
        {
            base.FireEvents(container);
            if (NeedFireEvent(3) && OwnerNodeId__Changed != null)
            {
                PropertyAddress __propAddress__ = EntityPropertyResolver.GetPropertyAddress(this);
                __propAddress__.DeltaObjectFieldId = 3;
                var __Event__Args__ = new EntityEventArgs(_OwnerNodeId, nameof(OwnerNodeId), __propAddress__, this);
                global::GeneratedCode.EntitySystem.EventHelper.FireEvent(container, __Event__Args__, OwnerNodeId__Changed);
            }

            if (NeedFireEvent(4) && ChainCalls__Changed != null)
            {
                PropertyAddress __propAddress__ = EntityPropertyResolver.GetPropertyAddress(this);
                __propAddress__.DeltaObjectFieldId = 4;
                var __Event__Args__ = new EntityEventArgs(_ChainCalls, nameof(ChainCalls), __propAddress__, this);
                global::GeneratedCode.EntitySystem.EventHelper.FireEvent(container, __Event__Args__, ChainCalls__Changed);
            }

            if (_ChainCalls != null && ((IDeltaObject)_ChainCalls).NeedFireEvents())
                ((IDeltaObject)_ChainCalls).ProcessEvents(container);
        }

        public override void Downgrade(long mask)
        {
            base.Downgrade(mask);
            if (((long)SharedCode.EntitySystem.ReplicationLevel.Master & mask) > 0)
            {
                ChainCalls = default;
            }

            if (_ChainCalls != null)
                ((IDeltaObjectExt)_ChainCalls).Downgrade(mask);
        }
    }
}

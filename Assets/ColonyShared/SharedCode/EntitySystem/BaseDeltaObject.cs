using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Assets.ColonyShared.GeneratedCode.Shared;
using Assets.ColonyShared.SharedCode.Utils;
using Core.Environment.Logging.Extension;
using GeneratedCode.Repositories;
using JetBrains.Annotations;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using NLog;
using ProtoBuf;
using SharedCode.Entities;
using SharedCode.Refs;
using SharedCode.Serializers;
using SharedCode.Serializers.Protobuf;
using GeneratedCode.DeltaObjects;
using SharedCode.Logging;
using GeneratedCode.EntitySystem;
using SharedCode.Repositories;
using ResourcesSystem.Base;

namespace SharedCode.EntitySystem
{
    [ProtoContract]
    public abstract partial class BaseDeltaObject : IDeltaObject, IDeltaObjectExt
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();
        public string TypeName => ReplicaTypeRegistry.GetTypeById(TypeId)?.GetFriendlyName() ?? "unknown:" + TypeId.ToString();

        private DirtyMask _mask;

        private long _replicationChangedMask = 0;
        
        protected long ReplicationChangedMask => _replicationChangedMask;

        [ProtoIgnore]
        [JsonIgnore]
        [BsonIgnore]
        public long LastParentEntityReplicationMask { get; set; }
        
        [ProtoIgnore]
        [JsonIgnore]
        [BsonIgnore]
        public short ParentEntityRefCount { get; set; }
        
        public virtual void IncrementParentRefs(IEntity parentEntity, bool trackChanged)
        {
            DeltaObjectHelper.IncrementParentRefs(this, parentEntity, trackChanged);
        }
        
        public virtual void DecrementParentRefs()
        {
            DeltaObjectHelper.DecrementParentRefs(this);
        }
        
        public bool HasParentRef()
        {
            return DeltaObjectHelper.HasParentRef(this);
        }

        public virtual void ReplicationLevelActualize(ReplicationLevel? actualParentLevel, ReplicationLevel? oldParentLevel)
        {
        }
        
        [ProtoIgnore]
        [JsonIgnore]
        [BsonIgnore]
        private IEntity _parentEntity;

        [ProtoIgnore]
        [JsonIgnore]
        [BsonIgnore]
        public IEntity parentEntity
        {
            get => _parentEntity;
            set => DeltaObjectHelper.ParentEntitySetterImpl(this, ref _parentEntity, value);
        }

        [ProtoIgnore]
        [JsonIgnore]
        [BsonIgnore]
        public IDeltaObjectExt parentDeltaObject { get; set; }

        private ulong _localId;
        [ProtoMember(2)]
        public ulong LocalId
        {
            get => _localId;
            set => DeltaObjectHelper.LocalIdSetterImpl(this, ref _localId, value);
        }

        [ProtoMember(3)]
        [JsonIgnore]
        [BsonIgnore]
        public DirtyMask Mask__Serialized
        {
            get => _mask;
            set => _mask = value;
        }

        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public bool Mask__SerializedSpecified
        {
            get => _SerializerContext.Pool.Current.FullSerialize || _mask.IsChanged();
        }

        [JsonIgnore]
        [ProtoIgnore]
        [BsonIgnore]
        public virtual int ParentTypeId => parentEntity?.TypeId ?? 0;

        [JsonIgnore]
        [ProtoIgnore]
        [BsonIgnore]
        public Guid ParentEntityId => parentEntity?.Id ?? Guid.Empty;

		[NotNull] //Boris: entity always IN a repo
        [JsonIgnore]
        [ProtoIgnore]
        [BsonIgnore]
        public virtual IEntitiesRepository EntitiesRepository
        {
            get { return parentEntity?.EntitiesRepository; }
            protected set { }
        }

        public abstract int TypeId { get; }

        public virtual Guid OwnerRepositoryId => ((BaseEntity) parentEntity)?.OwnerNodeId ?? Guid.Empty;

        public void SetDirty(int id, ReplicationLevel changedReplicationLevel)
        {
            _mask.SetDirty(id);
            SetDirtyReplicationMask(changedReplicationLevel);
            DeltaObjectHelper.AddChangedObject(parentEntity, this);
        }

        protected void SetDirtyReplicationMask(ReplicationLevel changedReplicationLevel)
        {
            long replicationLevelVal = (long)changedReplicationLevel;
            var val = (replicationLevelVal ^ (replicationLevelVal >> 1)) & replicationLevelVal;

            SetDirtyReplicationMask(val);
        }

        //public void Clear(int id)
        //{
        //    _mask &= ~((ulong)1 << id);
        //}

        public bool IsDirty(int id)
        {
            if (_SerializerContext.Pool.Current.FullSerialize)
                return true;

            return _mask.IsDirty(id);
        }

        public bool IsRequiredReplicationLevel(ReplicationLevel checkedReplicationLevel)
        {
            var currentMask = _SerializerContext.Pool.Current.ReplicationMask;
            long replicationLevelVal = (long)checkedReplicationLevel;
            var val = (replicationLevelVal ^ (replicationLevelVal >> 1)) & replicationLevelVal;
            bool result = (currentMask & val) == val;
            return result;
        }

        public bool NeedFireEvent(int id)
        {
            return _mask.IsDirty(id);
        }

        protected virtual void constructor()
        {
        }

        public bool NeedFireEvents()
        {
            return _replicationChangedMask > 0;
        }

        public virtual void ClearDelta()
        {
            _mask.ClearDelta();
            _replicationChangedMask = 0;
        }

        public virtual void GetAllLinkedEntities(long replicationMask,
            System.Collections.Generic.List<(long level, SharedCode.Refs.IEntityRef entityRef)> entities,
            long currentLevel,
            bool onlyDbEntities)
        {
        }

        public virtual void Visit(Action<IDeltaObject> visitor)
        {
            visitor(this);
        }
        
        public virtual void LinkEntityRefs(IEntitiesRepository repository)
        {
        }

        public virtual void FillReplicationSetRecursive(Dictionary<ReplicationLevel, Dictionary<IDeltaObject, DeltaObjectReplicationInfo>> replicationSets, HashSet<ReplicationLevel> traverseReplicationLevels, ReplicationLevel currentLevel, bool withBsonIgnore)
        {
            
        }

        public virtual void LinkChangedDeltaObjects(Dictionary<ulong, DeserializedObjectInfo> deserializedObjects, IEntity parentEntity)
        {
            
        }

        public virtual void SetParentDeltaObject(IDeltaObjectExt parentDeltaObject)
        {
            DeltaObjectHelper.SetParentDeltaObject(this, parentDeltaObject);
        }
        
        public virtual void Downgrade(long mask)
        {
        }

        protected virtual bool containsReplicationLevelInner(long mask)
        {
            if (parentEntity == null)
                return (LastParentEntityReplicationMask & mask) == mask;

            return ((BaseDeltaObject)parentEntity).containsReplicationLevelInner(mask);
        }

        public bool ContainsReplicationLevel(long mask)
        {
            return containsReplicationLevelInner(mask);
        }

        public void MarkAllChanged()
        {
            _mask.MarkAllChanged();
        }

        public void SubscribePropertyChanged(string propertyName, PropertyChangedDelegate callback)
        {
            Subscribe(propertyName, callback);
        }

        public void UnsubscribePropertyChanged(string propertyName, PropertyChangedDelegate callback)
        {
            Unsubscribe(propertyName, callback);
        }

        public void UnsubscribePropertyChanged(string propertyName)
        {
            Unsubscribe(propertyName);
        }

        public void UnsubscribeAll()
        {
            Unsubscribe();
        }

        protected virtual void Subscribe(string propertyName, PropertyChangedDelegate callback)
        {
        }

        protected virtual void Unsubscribe(string propertyName, PropertyChangedDelegate callback)
        {
        }

        protected virtual void Unsubscribe(string propertyName)
        {
        }

        protected virtual void Unsubscribe()
        {
        }

        public void SetDirtyReplicationMask(long val)
        {
            _replicationChangedMask |= val;
        }
        
        public bool IsReplicationMaskDirty(long checkedReplicationMask)
        {
            return (_replicationChangedMask & checkedReplicationMask) > 0;
        }

        public bool IsChanged()
        {
            return _replicationChangedMask > 0;
        }

        protected virtual void RandomFill(int __count__, Random __random__, bool withReadonly)
        {
        }

        void IHasRandomFill.Fill(int depthCount, Random random, bool withReadonly)
        {
            RandomFill(depthCount, random, withReadonly);
        }

        void ICanRandomFill.Fill(int depthCount, bool withReadonly)
        {
            Random random;
            using (var hasher = System.Security.Cryptography.SHA1.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(this.GetType().GetFriendlyName());
                var sha = hasher.ComputeHash(bytes);
                var hash = BitConverter.ToInt32(sha, 0);
                random = new Random(hash);
            }
            ((IHasRandomFill)this).Fill(depthCount, random, withReadonly);
        }

        void IDeltaObject.ProcessEvents(List<Func<Task>> container)
        {
            FireEvents(container);
        }

        protected virtual void FireEvents(List<Func<Task>> container)
        {
        }

        public virtual bool TryGetProperty<T>(int address, out T prop)
        {
            Log.Logger.IfError()?.Message("DeltaObject TryGetProperty not found {0} address {1}", this.GetType().Name, address).Write();
            prop = default(T);
            return false;
        }

        public virtual int GetIdOfChild(IDeltaObject deltaObject) => throw new InvalidOperationException($"Object {deltaObject} is not a child of {this}");
        public virtual int GetIdOfChildNonDeltaObj(string childName) => throw new InvalidOperationException($"Field {childName} is not a child of {this}");

        public abstract IDeltaObject GetReplicationLevel(ReplicationLevel replicationLevel);

        IEntity IDeltaObjectExt.GetParentEntity() => parentEntity;

        public IDeltaObjectExt GetParentObject() => parentDeltaObject;

        protected virtual void CopyValues()
        {
        }

        public override string ToString()
        {
            return string.Format("<DeltaObject {0}>", GetType().Name);
        }
    }

    [ProtoContract]
    public struct DirtyMask
    {
        public static DirtyMask AllChanged = new DirtyMask(ulong.MaxValue, ulong.MaxValue);

        [ProtoMember(1)]
        public ulong _mask;
        
        [ProtoMember(2)]
        public ulong _mask2;

        public DirtyMask(ulong mask, ulong mask2)
        {
            _mask = mask;
            _mask2 = mask2;
        }

        public void SetDirty(int id)
        {
            if (id < 64)
                _mask |= (ulong)1 << id;
            else
                _mask2 |= (ulong)1 << (id - 64);
        }

        public bool IsDirty(int id)
        {
            if (id < 64)
                return (_mask & (ulong)1 << id) != 0;
            return (_mask2 & (ulong)1 << (id - 64)) != 0;
        }

        public void ClearDelta()
        {
            _mask = 0;
            _mask2 = 0;
        }

        public void MarkAllChanged()
        {
            _mask = ulong.MaxValue;
            _mask2 = ulong.MaxValue;
        }

        public bool IsChanged()
        {
            return _mask != 0 || _mask2 != 0;
        }
    }
}

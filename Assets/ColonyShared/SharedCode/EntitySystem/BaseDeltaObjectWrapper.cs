using GeneratedCode.Repositories;
using SharedCode.Serializers.Protobuf;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SharedCode.Refs;
using SharedCode.Repositories;

namespace SharedCode.EntitySystem
{
    public abstract class BaseDeltaObjectWrapper: IBaseDeltaObjectWrapper, IDeltaObject
    {
        protected IDeltaObject __deltaObjectBase__;

        public int ParentTypeId => __deltaObjectBase__.ParentTypeId;
        public Guid ParentEntityId => __deltaObjectBase__.ParentEntityId;
        public IEntitiesRepository EntitiesRepository => __deltaObjectBase__.EntitiesRepository;
        public Guid OwnerRepositoryId => __deltaObjectBase__.OwnerRepositoryId;
        public abstract int TypeId { get; }
        public string TypeName => ReplicaTypeRegistry.GetTypeById(TypeId)?.GetFriendlyName() ?? "unknown:" + TypeId.ToString();

        public ulong LocalId => ((IDeltaObjectExt)__deltaObjectBase__).LocalId;

        protected BaseDeltaObjectWrapper(IDeltaObject deltaObject)
        {
            __deltaObjectBase__ = deltaObject;
        }

        public void Fill(int depthCount, Random random, bool withReadonly)
        {
            __deltaObjectBase__.Fill(depthCount, random, withReadonly);

        }

        public bool NeedFireEvents()
        {
            return __deltaObjectBase__.NeedFireEvents();
        }

        public void ClearDelta()
        {
            __deltaObjectBase__.ClearDelta();
        }

        public void GetAllLinkedEntities(long replicationMask, List<(long level, IEntityRef entityRef)> entities, long currentLevel, bool onlyDbEntities)
        {
             __deltaObjectBase__.GetAllLinkedEntities(replicationMask, entities, currentLevel, onlyDbEntities);
        }
        
        public void LinkEntityRefsRecursive(IEntitiesRepository _)
        {
            throw new NotImplementedException();
        }


        public void MarkAllChanged()
        {
            throw new NotImplementedException();
        }

        public void SubscribePropertyChanged(string propertyName, PropertyChangedDelegate callback)
        {
            __deltaObjectBase__.SubscribePropertyChanged(propertyName, callback);
        }

        public void UnsubscribePropertyChanged(string propertyName, PropertyChangedDelegate callback)
        {
            __deltaObjectBase__.UnsubscribePropertyChanged(propertyName, callback);
        }

        public void UnsubscribePropertyChanged(string propertyName)
        {
            __deltaObjectBase__.UnsubscribePropertyChanged(propertyName);
        }

        public void UnsubscribeAll()
        {
            __deltaObjectBase__.UnsubscribeAll();
        }

        public bool IsChanged()
        {
            throw new NotImplementedException();
        }

        public void ProcessEvents(List<Func<Task>> container)
        {
            __deltaObjectBase__.ProcessEvents(container);
        }

        public virtual bool TryGetProperty<T>(int address, out T property)
        {
            property = default(T);
            return false;
        }

        public virtual bool TryGetPropertyAddress(out PropertyAddress address, params string[] properties)
        {
            address = null;
            return false;
        }

        public virtual int GetIdOfChild(IDeltaObject deltaObject)
        {
            return -1;
        }

        public IDeltaObject GetReplicationLevel(ReplicationLevel replicationLevel)
        {
            return __deltaObjectBase__.GetReplicationLevel(replicationLevel);
        }

        IDeltaObject IBaseDeltaObjectWrapper.GetBaseDeltaObject()
        {
            return __deltaObjectBase__;
        }

        public void Fill(int depthCount, bool withReadonly)
        {
            __deltaObjectBase__.Fill(depthCount, withReadonly);
        }
    }
}

//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using SharedCode.Logging;
//using GeneratedCode.Manual.Repositories;
//using GeneratedCode.Repositories;
//using SharedCode.Refs;

//namespace SharedCode.EntitySystem
//{
//    public class EntitySingleThreadWrapper<T> : IEntitySingleThreadWrapper<T> where T : IEntity
//    {
//        public Guid Id { get; } = Guid.NewGuid();

//        private EntitiesContainerItem _item;

//        public IEntityRef EntityRef
//        {
//            get { return _item.EntityRef; }
//        }

//        private IEntitiesRepository _repository;

//        public T Entity
//        {
//            get
//            {
//                var typeId = EntitiesRepositoryBase.GetIdByType(typeof(T));
//                var replicationMask = EntitiesRepositoryBase.GetReplicationMask(typeId);
//                if (!EntityRef.ContainsReplicationLevel(replicationMask))
//                {
//                    Log.Logger.IfError()?.Message("Entity {0} typeId {1} not contains replication mask {2}", EntityRef.GetEntity().Id, EntityRef.GetEntity().TypeId, replicationMask).Write();
//                    return default(T);
//                }
//                return (T)EntitiesRepositoryBase.GetEntityReplicationLevel(typeId, EntityRef.GetEntity());
//            }
//        }

//        public T1 EntityAs<T1>()
//        {
//            var typeId = EntitiesRepositoryBase.GetIdByType(typeof(T1));
//            var replicationMask = EntitiesRepositoryBase.GetReplicationMask(typeId);
//            if (!EntityRef.ContainsReplicationLevel(replicationMask))
//            {
//                Log.Logger.IfError()?.Message("Entity {0} typeId {1} not contains replication mask {2}", EntityRef.GetEntity().Id, EntityRef.GetEntity().TypeId, replicationMask).Write();
//                return default(T1);
//            }
//            return (T1)EntitiesRepositoryBase.GetEntityReplicationLevel(typeId, EntityRef.GetEntity());
//        }

//        public EntitySingleThreadWrapper(IEntityRef entityRef, ReadWriteEntityOperationType operationType)
//        {
//            _item = new EntitiesContainerItem(entityRef, EntitiesRepositoryBase.GetIdByType(typeof(T)), operationType);
//            _repository = entityRef.GetEntity().EntitiesRepository;
//        }

//        public void Dispose()
//        {
//            Release();
//            ((IEntitiesRepositoryDataExtension)_repository).PopFromCurrentEntitiesWrapper();
//        }

//        public void Release()
//        {
//            if (_item != null)
//                ((IEntitiesRepositoryDataExtension)_repository).Release(_item);
//        }

//        public async Task LockAgain()
//        {
//            if (_item.OperationType == ReadWriteEntityOperationType.Read)
//                using (var container = await ((IEntitiesRepository)EntityRef.GetEntity().EntitiesRepository).GetRead<T>(_item.EntityRef.Id))
//                {
//                    _item = ((EntitySingleThreadWrapper<T>)container)._item;
//                    ((EntitySingleThreadWrapper<T>)container)._item = null;
//                }
//            else if (_item.OperationType == ReadWriteEntityOperationType.Write)
//                using (var container = await ((IEntitiesRepository)EntityRef.GetEntity().EntitiesRepository).GetWrite<T>(_item.EntityRef.Id))
//                {
//                    _item = ((EntitySingleThreadWrapper<T>)container)._item;
//                    ((EntitySingleThreadWrapper<T>)container)._item = null;
//                }
//        }
//    }

//    public class EntitySingleThreadWrapper : IEntitySingleThreadWrapper
//    {
//        public Guid Id { get; } = Guid.NewGuid();

//        private EntitiesContainerItem _item;

//        private int _typeId;

//        private IEntitiesRepository _repository;

//        public IEntityRef EntityRef
//        {
//            get { return _item.EntityRef; }
//        }

//        public IEntity Entity
//        {
//            get
//            {
//                var replicationMask = EntitiesRepositoryBase.GetReplicationMask(_typeId);
//                if (!EntityRef.ContainsReplicationLevel(replicationMask))
//                {
//                    Log.Logger.IfError()?.Message("Entity {0} typeId {1} not contains replication mask {2}", EntityRef.GetEntity().Id, EntityRef.GetEntity().TypeId, replicationMask).Write();
//                    return null;
//                }
//                return (IEntity)EntitiesRepositoryBase.GetEntityReplicationLevel(_typeId, EntityRef.GetEntity());
//            }
//        }

//        public T1 EntityAs<T1>()
//        {
//            var typeId = EntitiesRepositoryBase.GetIdByType(typeof(T1));
//            var replicationMask = EntitiesRepositoryBase.GetReplicationMask(typeId);
//            if (!EntityRef.ContainsReplicationLevel(replicationMask))
//            {
//                Log.Logger.IfError()?.Message("Entity {0} typeId {1} not contains replication mask {2}", EntityRef.GetEntity().Id, EntityRef.GetEntity().TypeId, replicationMask).Write();
//                return default(T1);
//            }
//            return (T1)EntitiesRepositoryBase.GetEntityReplicationLevel(_typeId, EntityRef.GetEntity());
//        }

//        public EntitySingleThreadWrapper(IEntityRef entityRef, int typeId, ReadWriteEntityOperationType operationType)
//        {
//            _item = new EntitiesContainerItem(entityRef, EntitiesRepositoryBase.GetIdByType(entityRef.GetEntityInterfaceType()), operationType);
//            _typeId = typeId;
//            _repository = entityRef.GetEntity().EntitiesRepository;
//        }

//        public void Dispose()
//        {
//            Release();
//            ((IEntitiesRepositoryDataExtension)_repository).PopFromCurrentEntitiesWrapper();
//        }

//        public void Release()
//        {
//            if (_item != null)
//                ((IEntitiesRepositoryDataExtension)_repository).Release(_item);
//        }

//        public async Task LockAgain()
//        {
//            var typeId = _item.EntityInterfaceTypeId;
//            if (_item.OperationType == ReadWriteEntityOperationType.Read)
//                using (var container = await ((IEntitiesRepository)EntityRef.GetEntity().EntitiesRepository).GetRead(typeId, _item.EntityRef.Id))
//                {
//                    _item = ((EntitySingleThreadWrapper)container)._item;
//                    ((EntitySingleThreadWrapper)container)._item = null;
//                }
//            else if (_item.OperationType == ReadWriteEntityOperationType.Write)
//                using (var container = await ((IEntitiesRepository)EntityRef.GetEntity().EntitiesRepository).GetWrite(typeId, _item.EntityRef.Id))
//                {
//                    _item = ((EntitySingleThreadWrapper)container)._item;
//                    ((EntitySingleThreadWrapper)container)._item = null;
//                }
//        }
//    }
//}

//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using SharedCode.Refs;

//namespace SharedCode.EntitySystem
//{
//    public interface IEntitySingleThreadWrapper<T> : IEntitiesRepositoryContainerWrapper, IDisposable where T : IEntity
//    {
//        IEntityRef EntityRef { get; }

//        T Entity { get; }

//        T1 EntityAs<T1>();
//    }

//    public interface IEntitySingleThreadWrapper : IEntitiesRepositoryContainerWrapper, IDisposable
//    {
//        IEntityRef EntityRef { get; }

//        IEntity Entity { get; }

//        T1 EntityAs<T1>();
//    }

//    public interface IEntitiesRepositoryContainerWrapper
//    {
//        Guid Id { get; }
//        void Release();
//        Task LockAgain();
//        IEntityBatch GetBatch();
//        T Get<T>(Guid entityId) where T : IEntity;
//        T Get<T>(int typeId, Guid entityId);
//        void CopyOldValues();
//    }
//}

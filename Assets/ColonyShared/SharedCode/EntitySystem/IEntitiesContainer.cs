using System;
using System.Threading.Tasks;
using ResourceSystem.Utils;
using SharedCode.Refs;

namespace SharedCode.EntitySystem
{
    public interface IEntitiesContainer : IDisposable
    {
        T Get<T>(int typeId, Guid entityId, ReplicationLevel replicationLevel);
        bool TryGet<T>(int typeId, Guid entityId, ReplicationLevel replicationLevel, out T entity);
        Task<T> GetOrSubscribe<T>(int typeId, Guid entityId, ReplicationLevel replicationLevel);
        IEntityRef GetEntityRef(int typeId, Guid entityId);
        bool IsEntityExist(int typeId, Guid entityId);
    }
}

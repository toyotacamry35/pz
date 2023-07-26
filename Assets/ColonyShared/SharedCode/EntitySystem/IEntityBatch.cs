using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SharedCode.EntitySystem
{
    public interface IEntityBatch
    {
        IEntityBatch Add<T>(Guid entityId, [CallerMemberName] string callerTag = "") where T : IEntity;
        IEntityBatch Add(int typeId, Guid entityId, [CallerMemberName] string callerTag = "");
        IEntityBatch Add(OuterRef<IEntity> outerRef, ReplicationLevel replicationLevel, [CallerMemberName] string callerTag = "");
        bool HasItem<T>(Guid entityId) where T : IEntity;
        bool HasItem(int typeId, Guid entityId);

        bool Empty { get; }
    }
}

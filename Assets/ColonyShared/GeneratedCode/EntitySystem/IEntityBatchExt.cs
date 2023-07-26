using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using SharedCode.EntitySystem;

namespace GeneratedCode.EntitySystem
{
    public interface IEntityBatchExt
    {
        IEntityBatch AddExclusive<T>(Guid entityId, [CallerMemberName] string callerTag = "") where T : IEntity;
        IEntityBatch AddExclusive(int typeId, Guid entityId, [CallerMemberName] string callerTag = "");
        IEntityBatch AddExclusiveTag<T>(Guid entityId, string callerTag) where T : IEntity;
        IEntityBatch AddExclusiveTag(int typeId, Guid entityId, string callerTag);
        IEntityBatch AddTag<T>(Guid entityId, string callerTag) where T : IEntity;
        IEntityBatch AddTag(int typeId, Guid entityId, string callerTag);
        bool HasWriteItem();
    }
}

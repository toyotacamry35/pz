using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedCode.EntitySystem
{
    public class EntityMethodCallTypeAttribute: Attribute
    {
        public EntityMethodCallType CallType { get; }

        public EntityMethodCallTypeAttribute(EntityMethodCallType callType)
        {
            CallType = callType;
        }
    }

    public enum EntityMethodCallType
    {
        Mutable,
        Immutable,
        ImmutableLocal,
        Lockfree,
        LockfreeLocal
    }
}

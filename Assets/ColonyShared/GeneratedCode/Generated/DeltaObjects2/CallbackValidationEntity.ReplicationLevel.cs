// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using SharedCode.Logging;
using GeneratedCode.Repositories;
using SharedCode.EntitySystem;
using GeneratedCode.EntitySystem;
using SharedCode.EntitySystem.Delta;

namespace GeneratedCode.DeltaObjects
{
    public class CallbackValidationEntityAlways : BaseEntityWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.ICallbackValidationEntityAlways
    {
        public CallbackValidationEntityAlways(SharedCode.Entities.Test.ICallbackValidationEntity deltaObject): base(deltaObject)
        {
        }

        SharedCode.Entities.Test.ICallbackValidationEntity __deltaObject__
        {
            get
            {
                return (SharedCode.Entities.Test.ICallbackValidationEntity)__deltaObjectBase__;
            }
        }

        public override int TypeId => -1659855269;
    }

    public class CallbackValidationEntityClientBroadcast : BaseEntityWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.ICallbackValidationEntityClientBroadcast
    {
        public CallbackValidationEntityClientBroadcast(SharedCode.Entities.Test.ICallbackValidationEntity deltaObject): base(deltaObject)
        {
        }

        SharedCode.Entities.Test.ICallbackValidationEntity __deltaObject__
        {
            get
            {
                return (SharedCode.Entities.Test.ICallbackValidationEntity)__deltaObjectBase__;
            }
        }

        public override int TypeId => 841583707;
    }

    public class CallbackValidationEntityClientFullApi : BaseEntityWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.ICallbackValidationEntityClientFullApi
    {
        public CallbackValidationEntityClientFullApi(SharedCode.Entities.Test.ICallbackValidationEntity deltaObject): base(deltaObject)
        {
        }

        SharedCode.Entities.Test.ICallbackValidationEntity __deltaObject__
        {
            get
            {
                return (SharedCode.Entities.Test.ICallbackValidationEntity)__deltaObjectBase__;
            }
        }

        public override int TypeId => 37768286;
    }

    public class CallbackValidationEntityClientFull : BaseEntityWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.ICallbackValidationEntityClientFull
    {
        public CallbackValidationEntityClientFull(SharedCode.Entities.Test.ICallbackValidationEntity deltaObject): base(deltaObject)
        {
        }

        SharedCode.Entities.Test.ICallbackValidationEntity __deltaObject__
        {
            get
            {
                return (SharedCode.Entities.Test.ICallbackValidationEntity)__deltaObjectBase__;
            }
        }

        public override int TypeId => -1940373984;
    }

    public class CallbackValidationEntityServerApi : BaseEntityWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.ICallbackValidationEntityServerApi
    {
        public CallbackValidationEntityServerApi(SharedCode.Entities.Test.ICallbackValidationEntity deltaObject): base(deltaObject)
        {
        }

        SharedCode.Entities.Test.ICallbackValidationEntity __deltaObject__
        {
            get
            {
                return (SharedCode.Entities.Test.ICallbackValidationEntity)__deltaObjectBase__;
            }
        }

        public override int TypeId => -1346701768;
    }

    public class CallbackValidationEntityServer : BaseEntityWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.ICallbackValidationEntityServer
    {
        public CallbackValidationEntityServer(SharedCode.Entities.Test.ICallbackValidationEntity deltaObject): base(deltaObject)
        {
        }

        SharedCode.Entities.Test.ICallbackValidationEntity __deltaObject__
        {
            get
            {
                return (SharedCode.Entities.Test.ICallbackValidationEntity)__deltaObjectBase__;
            }
        }

        public override int TypeId => -878936024;
    }
}
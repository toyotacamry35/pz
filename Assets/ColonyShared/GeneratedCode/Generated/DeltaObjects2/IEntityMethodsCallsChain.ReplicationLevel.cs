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
    public class EntityMethodsCallsChainAlways : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IEntityMethodsCallsChainAlways
    {
        public EntityMethodsCallsChainAlways(SharedCode.Entities.Core.IEntityMethodsCallsChain deltaObject): base(deltaObject)
        {
        }

        SharedCode.Entities.Core.IEntityMethodsCallsChain __deltaObject__
        {
            get
            {
                return (SharedCode.Entities.Core.IEntityMethodsCallsChain)__deltaObjectBase__;
            }
        }

        public System.Guid Id => __deltaObject__.Id;
        public override bool TryGetProperty<T>(int address, out T property)
        {
            property = default(T);
            object currProperty = null;
            switch (address)
            {
                case 16:
                    currProperty = Id;
                    break;
                default:
                    Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message("DeltaObject TryGetProperty not found {0} address {1}", this.GetType().Name, address).Write();
                    return false;
            }

            property = (T)currProperty;
            return true;
        }

        public override int TypeId => 1757055087;
    }

    public class EntityMethodsCallsChainClientBroadcast : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IEntityMethodsCallsChainClientBroadcast
    {
        public EntityMethodsCallsChainClientBroadcast(SharedCode.Entities.Core.IEntityMethodsCallsChain deltaObject): base(deltaObject)
        {
        }

        SharedCode.Entities.Core.IEntityMethodsCallsChain __deltaObject__
        {
            get
            {
                return (SharedCode.Entities.Core.IEntityMethodsCallsChain)__deltaObjectBase__;
            }
        }

        public System.Guid Id => __deltaObject__.Id;
        public override bool TryGetProperty<T>(int address, out T property)
        {
            property = default(T);
            object currProperty = null;
            switch (address)
            {
                case 16:
                    currProperty = Id;
                    break;
                default:
                    Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message("DeltaObject TryGetProperty not found {0} address {1}", this.GetType().Name, address).Write();
                    return false;
            }

            property = (T)currProperty;
            return true;
        }

        public override int TypeId => -1383318462;
    }

    public class EntityMethodsCallsChainClientFullApi : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IEntityMethodsCallsChainClientFullApi
    {
        public EntityMethodsCallsChainClientFullApi(SharedCode.Entities.Core.IEntityMethodsCallsChain deltaObject): base(deltaObject)
        {
        }

        SharedCode.Entities.Core.IEntityMethodsCallsChain __deltaObject__
        {
            get
            {
                return (SharedCode.Entities.Core.IEntityMethodsCallsChain)__deltaObjectBase__;
            }
        }

        public override int TypeId => 542303343;
    }

    public class EntityMethodsCallsChainClientFull : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IEntityMethodsCallsChainClientFull
    {
        public EntityMethodsCallsChainClientFull(SharedCode.Entities.Core.IEntityMethodsCallsChain deltaObject): base(deltaObject)
        {
        }

        SharedCode.Entities.Core.IEntityMethodsCallsChain __deltaObject__
        {
            get
            {
                return (SharedCode.Entities.Core.IEntityMethodsCallsChain)__deltaObjectBase__;
            }
        }

        public System.Guid Id => __deltaObject__.Id;
        public override bool TryGetProperty<T>(int address, out T property)
        {
            property = default(T);
            object currProperty = null;
            switch (address)
            {
                case 16:
                    currProperty = Id;
                    break;
                default:
                    Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message("DeltaObject TryGetProperty not found {0} address {1}", this.GetType().Name, address).Write();
                    return false;
            }

            property = (T)currProperty;
            return true;
        }

        public override int TypeId => 321301122;
    }

    public class EntityMethodsCallsChainServerApi : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IEntityMethodsCallsChainServerApi
    {
        public EntityMethodsCallsChainServerApi(SharedCode.Entities.Core.IEntityMethodsCallsChain deltaObject): base(deltaObject)
        {
        }

        SharedCode.Entities.Core.IEntityMethodsCallsChain __deltaObject__
        {
            get
            {
                return (SharedCode.Entities.Core.IEntityMethodsCallsChain)__deltaObjectBase__;
            }
        }

        public override int TypeId => -1032627748;
    }

    public class EntityMethodsCallsChainServer : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IEntityMethodsCallsChainServer
    {
        public EntityMethodsCallsChainServer(SharedCode.Entities.Core.IEntityMethodsCallsChain deltaObject): base(deltaObject)
        {
        }

        SharedCode.Entities.Core.IEntityMethodsCallsChain __deltaObject__
        {
            get
            {
                return (SharedCode.Entities.Core.IEntityMethodsCallsChain)__deltaObjectBase__;
            }
        }

        public System.Guid Id => __deltaObject__.Id;
        public override bool TryGetProperty<T>(int address, out T property)
        {
            property = default(T);
            object currProperty = null;
            switch (address)
            {
                case 16:
                    currProperty = Id;
                    break;
                default:
                    Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message("DeltaObject TryGetProperty not found {0} address {1}", this.GetType().Name, address).Write();
                    return false;
            }

            property = (T)currProperty;
            return true;
        }

        public override int TypeId => -1108018785;
    }
}
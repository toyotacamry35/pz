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
    public class AiTargetRecipientAlways : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IAiTargetRecipientAlways
    {
        public AiTargetRecipientAlways(SharedCode.AI.IAiTargetRecipient deltaObject): base(deltaObject)
        {
        }

        SharedCode.AI.IAiTargetRecipient __deltaObject__
        {
            get
            {
                return (SharedCode.AI.IAiTargetRecipient)__deltaObjectBase__;
            }
        }

        public ResourceSystem.Utils.OuterRef Target => __deltaObject__.Target;
        public override bool TryGetProperty<T>(int address, out T property)
        {
            property = default(T);
            object currProperty = null;
            switch (address)
            {
                case 10:
                    currProperty = Target;
                    break;
                default:
                    Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message("DeltaObject TryGetProperty not found {0} address {1}", this.GetType().Name, address).Write();
                    return false;
            }

            property = (T)currProperty;
            return true;
        }

        public override int TypeId => 612668507;
    }

    public class AiTargetRecipientClientBroadcast : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IAiTargetRecipientClientBroadcast
    {
        public AiTargetRecipientClientBroadcast(SharedCode.AI.IAiTargetRecipient deltaObject): base(deltaObject)
        {
        }

        SharedCode.AI.IAiTargetRecipient __deltaObject__
        {
            get
            {
                return (SharedCode.AI.IAiTargetRecipient)__deltaObjectBase__;
            }
        }

        public ResourceSystem.Utils.OuterRef Target => __deltaObject__.Target;
        public override bool TryGetProperty<T>(int address, out T property)
        {
            property = default(T);
            object currProperty = null;
            switch (address)
            {
                case 10:
                    currProperty = Target;
                    break;
                default:
                    Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message("DeltaObject TryGetProperty not found {0} address {1}", this.GetType().Name, address).Write();
                    return false;
            }

            property = (T)currProperty;
            return true;
        }

        public override int TypeId => -57449774;
    }

    public class AiTargetRecipientClientFullApi : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IAiTargetRecipientClientFullApi
    {
        public AiTargetRecipientClientFullApi(SharedCode.AI.IAiTargetRecipient deltaObject): base(deltaObject)
        {
        }

        SharedCode.AI.IAiTargetRecipient __deltaObject__
        {
            get
            {
                return (SharedCode.AI.IAiTargetRecipient)__deltaObjectBase__;
            }
        }

        public override int TypeId => -104341211;
    }

    public class AiTargetRecipientClientFull : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IAiTargetRecipientClientFull
    {
        public AiTargetRecipientClientFull(SharedCode.AI.IAiTargetRecipient deltaObject): base(deltaObject)
        {
        }

        SharedCode.AI.IAiTargetRecipient __deltaObject__
        {
            get
            {
                return (SharedCode.AI.IAiTargetRecipient)__deltaObjectBase__;
            }
        }

        public ResourceSystem.Utils.OuterRef Target => __deltaObject__.Target;
        public override bool TryGetProperty<T>(int address, out T property)
        {
            property = default(T);
            object currProperty = null;
            switch (address)
            {
                case 10:
                    currProperty = Target;
                    break;
                default:
                    Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message("DeltaObject TryGetProperty not found {0} address {1}", this.GetType().Name, address).Write();
                    return false;
            }

            property = (T)currProperty;
            return true;
        }

        public override int TypeId => -1487318655;
    }

    public class AiTargetRecipientServerApi : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IAiTargetRecipientServerApi
    {
        public AiTargetRecipientServerApi(SharedCode.AI.IAiTargetRecipient deltaObject): base(deltaObject)
        {
        }

        SharedCode.AI.IAiTargetRecipient __deltaObject__
        {
            get
            {
                return (SharedCode.AI.IAiTargetRecipient)__deltaObjectBase__;
            }
        }

        public override int TypeId => 885665461;
    }

    public class AiTargetRecipientServer : BaseDeltaObjectWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IAiTargetRecipientServer
    {
        public AiTargetRecipientServer(SharedCode.AI.IAiTargetRecipient deltaObject): base(deltaObject)
        {
        }

        SharedCode.AI.IAiTargetRecipient __deltaObject__
        {
            get
            {
                return (SharedCode.AI.IAiTargetRecipient)__deltaObjectBase__;
            }
        }

        public ResourceSystem.Utils.OuterRef Target => __deltaObject__.Target;
        public override bool TryGetProperty<T>(int address, out T property)
        {
            property = default(T);
            object currProperty = null;
            switch (address)
            {
                case 10:
                    currProperty = Target;
                    break;
                default:
                    Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message("DeltaObject TryGetProperty not found {0} address {1}", this.GetType().Name, address).Write();
                    return false;
            }

            property = (T)currProperty;
            return true;
        }

        public override int TypeId => 649618457;
    }
}
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
    public class VisibilityEntityAlways : BaseEntityWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IVisibilityEntityAlways
    {
        public VisibilityEntityAlways(SharedCode.MovementSync.IVisibilityEntity deltaObject): base(deltaObject)
        {
        }

        SharedCode.MovementSync.IVisibilityEntity __deltaObject__
        {
            get
            {
                return (SharedCode.MovementSync.IVisibilityEntity)__deltaObjectBase__;
            }
        }

        public override int TypeId => 2065916505;
    }

    public class VisibilityEntityClientBroadcast : BaseEntityWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IVisibilityEntityClientBroadcast
    {
        public VisibilityEntityClientBroadcast(SharedCode.MovementSync.IVisibilityEntity deltaObject): base(deltaObject)
        {
        }

        SharedCode.MovementSync.IVisibilityEntity __deltaObject__
        {
            get
            {
                return (SharedCode.MovementSync.IVisibilityEntity)__deltaObjectBase__;
            }
        }

        public override int TypeId => 199995731;
    }

    public class VisibilityEntityClientFullApi : BaseEntityWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IVisibilityEntityClientFullApi
    {
        public VisibilityEntityClientFullApi(SharedCode.MovementSync.IVisibilityEntity deltaObject): base(deltaObject)
        {
        }

        SharedCode.MovementSync.IVisibilityEntity __deltaObject__
        {
            get
            {
                return (SharedCode.MovementSync.IVisibilityEntity)__deltaObjectBase__;
            }
        }

        public override int TypeId => 608008491;
    }

    public class VisibilityEntityClientFull : BaseEntityWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IVisibilityEntityClientFull
    {
        public VisibilityEntityClientFull(SharedCode.MovementSync.IVisibilityEntity deltaObject): base(deltaObject)
        {
        }

        SharedCode.MovementSync.IVisibilityEntity __deltaObject__
        {
            get
            {
                return (SharedCode.MovementSync.IVisibilityEntity)__deltaObjectBase__;
            }
        }

        public override int TypeId => 815906327;
    }

    public class VisibilityEntityServerApi : BaseEntityWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IVisibilityEntityServerApi
    {
        public VisibilityEntityServerApi(SharedCode.MovementSync.IVisibilityEntity deltaObject): base(deltaObject)
        {
        }

        SharedCode.MovementSync.IVisibilityEntity __deltaObject__
        {
            get
            {
                return (SharedCode.MovementSync.IVisibilityEntity)__deltaObjectBase__;
            }
        }

        public override int TypeId => -191043229;
    }

    public class VisibilityEntityServer : BaseEntityWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IVisibilityEntityServer
    {
        public VisibilityEntityServer(SharedCode.MovementSync.IVisibilityEntity deltaObject): base(deltaObject)
        {
        }

        SharedCode.MovementSync.IVisibilityEntity __deltaObject__
        {
            get
            {
                return (SharedCode.MovementSync.IVisibilityEntity)__deltaObjectBase__;
            }
        }

        public System.Guid WorldSpace => __deltaObject__.WorldSpace;
        public System.Threading.Tasks.Task<bool> ForceUnsubscribeAll(System.Guid user)
        {
            return __deltaObject__.ForceUnsubscribeAll(user);
        }

        public override bool TryGetProperty<T>(int address, out T property)
        {
            property = default(T);
            object currProperty = null;
            switch (address)
            {
                case 10:
                    currProperty = WorldSpace;
                    break;
                default:
                    Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message("DeltaObject TryGetProperty not found {0} address {1}", this.GetType().Name, address).Write();
                    return false;
            }

            property = (T)currProperty;
            return true;
        }

        public override int TypeId => 521604314;
    }
}
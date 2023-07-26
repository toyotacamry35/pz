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
    public class SessionHolderEntityAlways : BaseEntityWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.ISessionHolderEntityAlways
    {
        public SessionHolderEntityAlways(GeneratedCode.Telemetry.ISessionHolderEntity deltaObject): base(deltaObject)
        {
        }

        GeneratedCode.Telemetry.ISessionHolderEntity __deltaObject__
        {
            get
            {
                return (GeneratedCode.Telemetry.ISessionHolderEntity)__deltaObjectBase__;
            }
        }

        public IDeltaDictionary<System.Guid, System.Guid> SessionsByGuid
        {
            get
            {
                return __deltaObject__.SessionsByGuid;
            }
        }

        public System.Threading.Tasks.ValueTask Register(System.Guid guid, System.Guid session)
        {
            return __deltaObject__.Register(guid, session);
        }

        public System.Threading.Tasks.ValueTask Unregister(System.Guid guid)
        {
            return __deltaObject__.Unregister(guid);
        }

        public override bool TryGetProperty<T>(int address, out T property)
        {
            property = default(T);
            object currProperty = null;
            switch (address)
            {
                case 10:
                    currProperty = SessionsByGuid;
                    break;
                default:
                    Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message("DeltaObject TryGetProperty not found {0} address {1}", this.GetType().Name, address).Write();
                    return false;
            }

            property = (T)currProperty;
            return true;
        }

        public override int TypeId => 597657531;
    }

    public class SessionHolderEntityClientBroadcast : BaseEntityWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.ISessionHolderEntityClientBroadcast
    {
        public SessionHolderEntityClientBroadcast(GeneratedCode.Telemetry.ISessionHolderEntity deltaObject): base(deltaObject)
        {
        }

        GeneratedCode.Telemetry.ISessionHolderEntity __deltaObject__
        {
            get
            {
                return (GeneratedCode.Telemetry.ISessionHolderEntity)__deltaObjectBase__;
            }
        }

        public IDeltaDictionary<System.Guid, System.Guid> SessionsByGuid
        {
            get
            {
                return __deltaObject__.SessionsByGuid;
            }
        }

        public System.Threading.Tasks.ValueTask Register(System.Guid guid, System.Guid session)
        {
            return __deltaObject__.Register(guid, session);
        }

        public System.Threading.Tasks.ValueTask Unregister(System.Guid guid)
        {
            return __deltaObject__.Unregister(guid);
        }

        public override bool TryGetProperty<T>(int address, out T property)
        {
            property = default(T);
            object currProperty = null;
            switch (address)
            {
                case 10:
                    currProperty = SessionsByGuid;
                    break;
                default:
                    Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message("DeltaObject TryGetProperty not found {0} address {1}", this.GetType().Name, address).Write();
                    return false;
            }

            property = (T)currProperty;
            return true;
        }

        public override int TypeId => 1944832119;
    }

    public class SessionHolderEntityClientFullApi : BaseEntityWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.ISessionHolderEntityClientFullApi
    {
        public SessionHolderEntityClientFullApi(GeneratedCode.Telemetry.ISessionHolderEntity deltaObject): base(deltaObject)
        {
        }

        GeneratedCode.Telemetry.ISessionHolderEntity __deltaObject__
        {
            get
            {
                return (GeneratedCode.Telemetry.ISessionHolderEntity)__deltaObjectBase__;
            }
        }

        public override int TypeId => -1124566629;
    }

    public class SessionHolderEntityClientFull : BaseEntityWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.ISessionHolderEntityClientFull
    {
        public SessionHolderEntityClientFull(GeneratedCode.Telemetry.ISessionHolderEntity deltaObject): base(deltaObject)
        {
        }

        GeneratedCode.Telemetry.ISessionHolderEntity __deltaObject__
        {
            get
            {
                return (GeneratedCode.Telemetry.ISessionHolderEntity)__deltaObjectBase__;
            }
        }

        public IDeltaDictionary<System.Guid, System.Guid> SessionsByGuid
        {
            get
            {
                return __deltaObject__.SessionsByGuid;
            }
        }

        public System.Threading.Tasks.ValueTask Register(System.Guid guid, System.Guid session)
        {
            return __deltaObject__.Register(guid, session);
        }

        public System.Threading.Tasks.ValueTask Unregister(System.Guid guid)
        {
            return __deltaObject__.Unregister(guid);
        }

        public override bool TryGetProperty<T>(int address, out T property)
        {
            property = default(T);
            object currProperty = null;
            switch (address)
            {
                case 10:
                    currProperty = SessionsByGuid;
                    break;
                default:
                    Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message("DeltaObject TryGetProperty not found {0} address {1}", this.GetType().Name, address).Write();
                    return false;
            }

            property = (T)currProperty;
            return true;
        }

        public override int TypeId => 1753201462;
    }

    public class SessionHolderEntityServerApi : BaseEntityWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.ISessionHolderEntityServerApi
    {
        public SessionHolderEntityServerApi(GeneratedCode.Telemetry.ISessionHolderEntity deltaObject): base(deltaObject)
        {
        }

        GeneratedCode.Telemetry.ISessionHolderEntity __deltaObject__
        {
            get
            {
                return (GeneratedCode.Telemetry.ISessionHolderEntity)__deltaObjectBase__;
            }
        }

        public override int TypeId => 1267468275;
    }

    public class SessionHolderEntityServer : BaseEntityWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.ISessionHolderEntityServer
    {
        public SessionHolderEntityServer(GeneratedCode.Telemetry.ISessionHolderEntity deltaObject): base(deltaObject)
        {
        }

        GeneratedCode.Telemetry.ISessionHolderEntity __deltaObject__
        {
            get
            {
                return (GeneratedCode.Telemetry.ISessionHolderEntity)__deltaObjectBase__;
            }
        }

        public IDeltaDictionary<System.Guid, System.Guid> SessionsByGuid
        {
            get
            {
                return __deltaObject__.SessionsByGuid;
            }
        }

        public System.Threading.Tasks.ValueTask Register(System.Guid guid, System.Guid session)
        {
            return __deltaObject__.Register(guid, session);
        }

        public System.Threading.Tasks.ValueTask Unregister(System.Guid guid)
        {
            return __deltaObject__.Unregister(guid);
        }

        public override bool TryGetProperty<T>(int address, out T property)
        {
            property = default(T);
            object currProperty = null;
            switch (address)
            {
                case 10:
                    currProperty = SessionsByGuid;
                    break;
                default:
                    Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message("DeltaObject TryGetProperty not found {0} address {1}", this.GetType().Name, address).Write();
                    return false;
            }

            property = (T)currProperty;
            return true;
        }

        public override int TypeId => 1559307416;
    }
}
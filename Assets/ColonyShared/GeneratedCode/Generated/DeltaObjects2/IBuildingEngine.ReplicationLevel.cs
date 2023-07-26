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
    public class BuildingEngineAlways : BaseEntityWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IBuildingEngineAlways
    {
        public BuildingEngineAlways(SharedCode.Entities.Engine.IBuildingEngine deltaObject): base(deltaObject)
        {
        }

        SharedCode.Entities.Engine.IBuildingEngine __deltaObject__
        {
            get
            {
                return (SharedCode.Entities.Engine.IBuildingEngine)__deltaObjectBase__;
            }
        }

        public GeneratedCode.DeltaObjects.ReplicationInterfaces.IOwnerInformationAlways OwnerInformation => (GeneratedCode.DeltaObjects.ReplicationInterfaces.IOwnerInformationAlways)__deltaObject__.OwnerInformation?.GetReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.Always);
        public override bool TryGetProperty<T>(int address, out T property)
        {
            property = default(T);
            object currProperty = null;
            switch (address)
            {
                case 10:
                    currProperty = OwnerInformation;
                    break;
                default:
                    Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message("DeltaObject TryGetProperty not found {0} address {1}", this.GetType().Name, address).Write();
                    return false;
            }

            property = (T)currProperty;
            return true;
        }

        public override int TypeId => -1358063728;
    }

    public class BuildingEngineClientBroadcast : BaseEntityWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IBuildingEngineClientBroadcast
    {
        public BuildingEngineClientBroadcast(SharedCode.Entities.Engine.IBuildingEngine deltaObject): base(deltaObject)
        {
        }

        SharedCode.Entities.Engine.IBuildingEngine __deltaObject__
        {
            get
            {
                return (SharedCode.Entities.Engine.IBuildingEngine)__deltaObjectBase__;
            }
        }

        public GeneratedCode.DeltaObjects.ReplicationInterfaces.IOwnerInformationClientBroadcast OwnerInformation => (GeneratedCode.DeltaObjects.ReplicationInterfaces.IOwnerInformationClientBroadcast)__deltaObject__.OwnerInformation?.GetReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast);
        public override bool TryGetProperty<T>(int address, out T property)
        {
            property = default(T);
            object currProperty = null;
            switch (address)
            {
                case 10:
                    currProperty = OwnerInformation;
                    break;
                default:
                    Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message("DeltaObject TryGetProperty not found {0} address {1}", this.GetType().Name, address).Write();
                    return false;
            }

            property = (T)currProperty;
            return true;
        }

        public override int TypeId => 1498828004;
    }

    public class BuildingEngineClientFullApi : BaseEntityWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IBuildingEngineClientFullApi
    {
        public BuildingEngineClientFullApi(SharedCode.Entities.Engine.IBuildingEngine deltaObject): base(deltaObject)
        {
        }

        SharedCode.Entities.Engine.IBuildingEngine __deltaObject__
        {
            get
            {
                return (SharedCode.Entities.Engine.IBuildingEngine)__deltaObjectBase__;
            }
        }

        public override int TypeId => 1174549955;
    }

    public class BuildingEngineClientFull : BaseEntityWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IBuildingEngineClientFull
    {
        public BuildingEngineClientFull(SharedCode.Entities.Engine.IBuildingEngine deltaObject): base(deltaObject)
        {
        }

        SharedCode.Entities.Engine.IBuildingEngine __deltaObject__
        {
            get
            {
                return (SharedCode.Entities.Engine.IBuildingEngine)__deltaObjectBase__;
            }
        }

        public GeneratedCode.DeltaObjects.ReplicationInterfaces.IOwnerInformationClientFull OwnerInformation => (GeneratedCode.DeltaObjects.ReplicationInterfaces.IOwnerInformationClientFull)__deltaObject__.OwnerInformation?.GetReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.ClientFull);
        public System.Threading.Tasks.Task<SharedCode.Entities.Engine.BuildOperationResult> Build(SharedCode.EntitySystem.PropertyAddress address, int slodIds, SharedCode.Utils.Vector3 position, SharedCode.Utils.Quaternion rotation)
        {
            return __deltaObject__.Build(address, slodIds, position, rotation);
        }

        public override bool TryGetProperty<T>(int address, out T property)
        {
            property = default(T);
            object currProperty = null;
            switch (address)
            {
                case 10:
                    currProperty = OwnerInformation;
                    break;
                default:
                    Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message("DeltaObject TryGetProperty not found {0} address {1}", this.GetType().Name, address).Write();
                    return false;
            }

            property = (T)currProperty;
            return true;
        }

        public override int TypeId => 1290109515;
    }

    public class BuildingEngineServerApi : BaseEntityWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IBuildingEngineServerApi
    {
        public BuildingEngineServerApi(SharedCode.Entities.Engine.IBuildingEngine deltaObject): base(deltaObject)
        {
        }

        SharedCode.Entities.Engine.IBuildingEngine __deltaObject__
        {
            get
            {
                return (SharedCode.Entities.Engine.IBuildingEngine)__deltaObjectBase__;
            }
        }

        public override int TypeId => -2011874020;
    }

    public class BuildingEngineServer : BaseEntityWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IBuildingEngineServer
    {
        public BuildingEngineServer(SharedCode.Entities.Engine.IBuildingEngine deltaObject): base(deltaObject)
        {
        }

        SharedCode.Entities.Engine.IBuildingEngine __deltaObject__
        {
            get
            {
                return (SharedCode.Entities.Engine.IBuildingEngine)__deltaObjectBase__;
            }
        }

        public GeneratedCode.DeltaObjects.ReplicationInterfaces.IOwnerInformationServer OwnerInformation => (GeneratedCode.DeltaObjects.ReplicationInterfaces.IOwnerInformationServer)__deltaObject__.OwnerInformation?.GetReplicationLevel(SharedCode.EntitySystem.ReplicationLevel.Server);
        public System.Threading.Tasks.Task<SharedCode.Entities.Engine.BuildOperationResult> Build(SharedCode.EntitySystem.PropertyAddress address, int slodIds, SharedCode.Utils.Vector3 position, SharedCode.Utils.Quaternion rotation)
        {
            return __deltaObject__.Build(address, slodIds, position, rotation);
        }

        public override bool TryGetProperty<T>(int address, out T property)
        {
            property = default(T);
            object currProperty = null;
            switch (address)
            {
                case 10:
                    currProperty = OwnerInformation;
                    break;
                default:
                    Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message("DeltaObject TryGetProperty not found {0} address {1}", this.GetType().Name, address).Write();
                    return false;
            }

            property = (T)currProperty;
            return true;
        }

        public override int TypeId => 1975211775;
    }
}
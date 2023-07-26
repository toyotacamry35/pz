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
    public class ClientCommunicationEntityAlways : BaseEntityWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IClientCommunicationEntityAlways
    {
        public ClientCommunicationEntityAlways(SharedCode.Entities.Cloud.IClientCommunicationEntity deltaObject): base(deltaObject)
        {
        }

        SharedCode.Entities.Cloud.IClientCommunicationEntity __deltaObject__
        {
            get
            {
                return (SharedCode.Entities.Cloud.IClientCommunicationEntity)__deltaObjectBase__;
            }
        }

        public System.Threading.Tasks.Task SetLevelLoaded()
        {
            return __deltaObject__.SetLevelLoaded();
        }

        public override int TypeId => -69122918;
    }

    public class ClientCommunicationEntityClientBroadcast : BaseEntityWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IClientCommunicationEntityClientBroadcast
    {
        public ClientCommunicationEntityClientBroadcast(SharedCode.Entities.Cloud.IClientCommunicationEntity deltaObject): base(deltaObject)
        {
        }

        SharedCode.Entities.Cloud.IClientCommunicationEntity __deltaObject__
        {
            get
            {
                return (SharedCode.Entities.Cloud.IClientCommunicationEntity)__deltaObjectBase__;
            }
        }

        public System.Threading.Tasks.Task SetLevelLoaded()
        {
            return __deltaObject__.SetLevelLoaded();
        }

        public override int TypeId => 1623454410;
    }

    public class ClientCommunicationEntityClientFullApi : BaseEntityWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IClientCommunicationEntityClientFullApi
    {
        public ClientCommunicationEntityClientFullApi(SharedCode.Entities.Cloud.IClientCommunicationEntity deltaObject): base(deltaObject)
        {
        }

        SharedCode.Entities.Cloud.IClientCommunicationEntity __deltaObject__
        {
            get
            {
                return (SharedCode.Entities.Cloud.IClientCommunicationEntity)__deltaObjectBase__;
            }
        }

        public override int TypeId => 142590145;
    }

    public class ClientCommunicationEntityClientFull : BaseEntityWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IClientCommunicationEntityClientFull
    {
        public ClientCommunicationEntityClientFull(SharedCode.Entities.Cloud.IClientCommunicationEntity deltaObject): base(deltaObject)
        {
        }

        SharedCode.Entities.Cloud.IClientCommunicationEntity __deltaObject__
        {
            get
            {
                return (SharedCode.Entities.Cloud.IClientCommunicationEntity)__deltaObjectBase__;
            }
        }

        public System.Threading.Tasks.Task SetLevelLoaded()
        {
            return __deltaObject__.SetLevelLoaded();
        }

        public override int TypeId => 480419988;
    }

    public class ClientCommunicationEntityServerApi : BaseEntityWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IClientCommunicationEntityServerApi
    {
        public ClientCommunicationEntityServerApi(SharedCode.Entities.Cloud.IClientCommunicationEntity deltaObject): base(deltaObject)
        {
        }

        SharedCode.Entities.Cloud.IClientCommunicationEntity __deltaObject__
        {
            get
            {
                return (SharedCode.Entities.Cloud.IClientCommunicationEntity)__deltaObjectBase__;
            }
        }

        public override int TypeId => -269342818;
    }

    public class ClientCommunicationEntityServer : BaseEntityWrapper, GeneratedCode.DeltaObjects.ReplicationInterfaces.IClientCommunicationEntityServer
    {
        public ClientCommunicationEntityServer(SharedCode.Entities.Cloud.IClientCommunicationEntity deltaObject): base(deltaObject)
        {
        }

        SharedCode.Entities.Cloud.IClientCommunicationEntity __deltaObject__
        {
            get
            {
                return (SharedCode.Entities.Cloud.IClientCommunicationEntity)__deltaObjectBase__;
            }
        }

        public bool LevelLoaded => __deltaObject__.LevelLoaded;
        public IDeltaList<SharedCode.Entities.Cloud.ConnectionInfo> Connections => __deltaObject__.Connections;
        public System.Threading.Tasks.Task SetLevelLoaded()
        {
            return __deltaObject__.SetLevelLoaded();
        }

        public System.Threading.Tasks.Task DisconnectByAnotherConnection()
        {
            return __deltaObject__.DisconnectByAnotherConnection();
        }

        public System.Threading.Tasks.Task GracefullLogout()
        {
            return __deltaObject__.GracefullLogout();
        }

        public System.Threading.Tasks.Task DisconnectByError(string reason, string details)
        {
            return __deltaObject__.DisconnectByError(reason, details);
        }

        public System.Threading.Tasks.Task ConfirmLogin()
        {
            return __deltaObject__.ConfirmLogin();
        }

        public System.Threading.Tasks.Task<SharedCode.Entities.Cloud.MapHostInitialInformation> GetMapHostInitialInformation()
        {
            return __deltaObject__.GetMapHostInitialInformation();
        }

        public System.Threading.Tasks.Task<bool> AddConection(string host, int port, System.Guid nodeId)
        {
            return __deltaObject__.AddConection(host, port, nodeId);
        }

        public System.Threading.Tasks.Task<bool> RemoveConection(System.Guid nodeId)
        {
            return __deltaObject__.RemoveConection(nodeId);
        }

        public override bool TryGetProperty<T>(int address, out T property)
        {
            property = default(T);
            object currProperty = null;
            switch (address)
            {
                case 10:
                    currProperty = LevelLoaded;
                    break;
                case 11:
                    currProperty = Connections;
                    break;
                default:
                    Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message("DeltaObject TryGetProperty not found {0} address {1}", this.GetType().Name, address).Write();
                    return false;
            }

            property = (T)currProperty;
            return true;
        }

        public override int TypeId => -2101219827;
    }
}
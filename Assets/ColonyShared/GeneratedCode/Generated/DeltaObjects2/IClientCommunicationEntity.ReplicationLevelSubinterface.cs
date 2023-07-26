// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects.ReplicationInterfaces
{
    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.Always, 244917423, typeof(SharedCode.Entities.Cloud.IClientCommunicationEntity))]
    public interface IClientCommunicationEntityAlways : SharedCode.EntitySystem.IEntity, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        System.Threading.Tasks.Task SetLevelLoaded();
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast, -116735253, typeof(SharedCode.Entities.Cloud.IClientCommunicationEntity))]
    public interface IClientCommunicationEntityClientBroadcast : SharedCode.EntitySystem.IEntity, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        System.Threading.Tasks.Task SetLevelLoaded();
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientFullApi, 224081125, typeof(SharedCode.Entities.Cloud.IClientCommunicationEntity))]
    public interface IClientCommunicationEntityClientFullApi : SharedCode.EntitySystem.IEntity, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ClientFull, 1528753857, typeof(SharedCode.Entities.Cloud.IClientCommunicationEntity))]
    public interface IClientCommunicationEntityClientFull : SharedCode.EntitySystem.IEntity, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        System.Threading.Tasks.Task SetLevelLoaded();
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.ServerApi, 1616318299, typeof(SharedCode.Entities.Cloud.IClientCommunicationEntity))]
    public interface IClientCommunicationEntityServerApi : SharedCode.EntitySystem.IEntity, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
    }

    [SharedCode.EntitySystem.ReplicationInterface(SharedCode.EntitySystem.ReplicationLevel.Server, 519675334, typeof(SharedCode.Entities.Cloud.IClientCommunicationEntity))]
    public interface IClientCommunicationEntityServer : SharedCode.EntitySystem.IEntity, SharedCode.EntitySystem.IBaseDeltaObjectWrapper
    {
        bool LevelLoaded
        {
            get;
        }

        SharedCode.EntitySystem.Delta.IDeltaList<SharedCode.Entities.Cloud.ConnectionInfo> Connections
        {
            get;
        }

        System.Threading.Tasks.Task SetLevelLoaded();
        System.Threading.Tasks.Task DisconnectByAnotherConnection();
        System.Threading.Tasks.Task GracefullLogout();
        System.Threading.Tasks.Task DisconnectByError(string reason, string details);
        System.Threading.Tasks.Task ConfirmLogin();
        System.Threading.Tasks.Task<SharedCode.Entities.Cloud.MapHostInitialInformation> GetMapHostInitialInformation();
        System.Threading.Tasks.Task<bool> AddConection(string host, int port, System.Guid nodeId);
        System.Threading.Tasks.Task<bool> RemoveConection(System.Guid nodeId);
    }
}